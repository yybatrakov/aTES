﻿//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/linq2db).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------

#pragma warning disable 1573, 1591

using System;
using System.Collections.Generic;

using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.Mapping;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace tasks.Api.Data
{
	/// <summary>
	/// Database       : popug
	/// Data Source    : tcp://localhost:5432
	/// Server Version : 13.3 (Debian 13.3-1.pgdg100+1)
	/// </summary>
	public partial class tasksDb : LinqToDB.Data.DataConnection
	{
		public virtual ITable<Task> Tasks { get { return this.GetTable<Task>(); } }

		public tasksDb(IConfiguration configuration)
			: base(new PostgreSQLDataProvider(PostgreSQLVersion.v95), configuration.GetConnectionString(nameof(tasksDb)))
		{
			//InitJsonMappingSchema();
		}

		public tasksDb(string connectionString)
			: base(new PostgreSQLDataProvider(PostgreSQLVersion.v95), connectionString)
		{
			//InitJsonMappingSchema();
		}

		protected void InitMappingSchema()
		{
		}
	}

	[Table(Schema="tasks", Name="tasks")]
	public partial class Task
	{
		[Column("task_id"),      PrimaryKey, Identity] public int    TaskId      { get; set; } // integer
		[Column("description"),  Nullable            ] public string Description { get; set; } // character varying(1024)
		[Column("is_completed"), Nullable            ] public bool?  IsCompleted { get; set; } // boolean
		[Column("assigned_to"),  Nullable            ] public string AssignedTo  { get; set; } // character varying(124)
	}
}
