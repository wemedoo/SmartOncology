using Serilog;
using sReportsV2.Common.Helpers;

namespace sReportsV2.Domain.DatabaseMigrationScripts
{
    public abstract class MongoMigration
    {
        public abstract int Version { get; }
        /// <summary>
        ///     Operations to be performed during the upgrade process.
        /// </summary>
        protected abstract void Up();

        /// <summary>
        ///     Operations to be performed during the downgrade process.
        /// </summary>
        protected abstract void Down();

        public void ExecuteUp()
        {
            LogHelper.Info($"Mongo Migration {this.GetType().Name} Upgrade started");
            Up();
            LogHelper.Info($"Mongo Migration {this.GetType().Name} Upgrade finished");
        }


        public void ExecuteDown()
        {
            LogHelper.Info($"Mongo Migration {this.GetType().Name} Downgrade started");
            Down();
            LogHelper.Info($"Mongo Migration {this.GetType().Name} Downgrade finished");
        }
    }
}
