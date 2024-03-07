using Microsoft.AspNetCore.Http;
using System;

namespace PopugCommon.KafkaMessages
{
    public static class Messages
    {
        public static class Tasks
        {
            public static class Stream
            {
                public const string Updated = "task.stream.updated";
                public const string Created = "task.stream.created";
            }
            public const string Completed = "task.completed";
            public const string Assigned = "task.assigned";
            public const string ReAssigned = "tasks.reassigned";
        }
        public static class Balances
        {
            public static class Stream
            {
                public const string Updated = "balance.stream.updated";
                public const string Created = "balance.stream.created";
            }
        }
        public static class BalanceTransaction
        {
            public static class Stream
            {
                public const string Created = "balance.transaction.stream.created";
            }
        }
        public static class Users
        {
            public static class Stream
            {
                public const string Updated = "users.stream.updated";
                public const string Created = "users.stream.created";
                public const string Deleted = "users.stream.deleted";
            }
        }
    }
}
