// See https://aka.ms/new-console-template for more information
using FlagMigrationToUnified;

var repo = new DAL();

await repo.MigrateFlagDetailsToUnified();

