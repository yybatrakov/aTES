using Confluent.Kafka;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using PopugTaskTracker;
using System;
using System.Threading.Tasks;

namespace PopugAccounting.Logic
{
    public class TasksLifecycleConsumer : KafkaConsumer
    {
        public override string MessageType => KafkaTopics.TasksLifecycle;

        public AccountingLogic AccountingLogic { get; }

        public TasksLifecycleConsumer(AccountingLogic accountingLogic)
        {
            AccountingLogic = accountingLogic;
        }

        public async override Task OnMessage(Message<Ignore, string> message)
        {
            var popug = SerializeExtensions.FromJson<PopugMessage>(message.Value);

            switch ($"{popug.Event}_{popug.Version}")
            {
                case Messages.Tasks.Assigned + "_v1":
                    var taskAssigned = SerializeExtensions.FromJson<TaskAssignedEvent>(popug.Data.ToString());
                    var task = await AccountingLogic.GetTask(taskAssigned.PublicId);
                    if (task == null)
                    {
                        task = new TaskDb()
                        {
                            PublicId = taskAssigned.PublicId,
                            AssignedUserId = taskAssigned.AssignedUserId
                        };
                        await AccountingLogic.AddOrUpdateTask(task);
                    }
                    await AccountingLogic.UpdateBalance(new BalanceTransactionDb() { Type = TransactionType.Assign, Date = popug.EventDate, Money = -task.Fee, UserId = taskAssigned.AssignedUserId });
                    break;
                case Messages.Tasks.ReAssigned + "_v1":
                    var tasksReassigned = SerializeExtensions.FromJson<TasksReassignedEvent>(popug.Data.ToString());
                    foreach (var t in tasksReassigned.Tasks)
                    {
                        task = await AccountingLogic.GetTask(t.PublicId);
                        await AccountingLogic.UpdateBalance(new BalanceTransactionDb() { Type = TransactionType.Assign, Date = popug.EventDate, Money = -task.Fee, UserId = t.AssignedUserId });
                    }
                    break;
                case Messages.Tasks.Completed + "_v1":
                    var taskCompleted = SerializeExtensions.FromJson<TaskCompletedEvent>(popug.Data.ToString());
                    task = await AccountingLogic.GetTask(taskCompleted.PublicId);
                    //TODO, если еще не назначили деньги
                    await AccountingLogic.UpdateBalance(new BalanceTransactionDb() { Type = TransactionType.Complete, Date = popug.EventDate, Money = task.Amount, UserId = task.AssignedUserId });
                    break;
            }
        }
    }
}
