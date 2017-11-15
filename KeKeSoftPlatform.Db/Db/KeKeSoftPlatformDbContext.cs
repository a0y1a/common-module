using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using KeKeSoftPlatform.Common;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Data.Common;
using System.Diagnostics;
using System.Web;

namespace KeKeSoftPlatform.Db
{
    public class KeKeSoftPlatformDbContext : DbContext
    {
        #region 事务
        public static ReturnValue<T> UseTransaction<T>(Func<KeKeSoftPlatformDbContext, ReturnValue<T>> action)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = action(db);
                    if (result.IsSuccess)
                    {
                        trans.Commit();
                    }
                    else
                    {
                        trans.Rollback();
                    }
                    return result;
                }
            }
        }

        public static ReturnValue UseTransaction(Func<KeKeSoftPlatformDbContext, ReturnValue> action)
        {
            using (KeKeSoftPlatformDbContext db = new KeKeSoftPlatformDbContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var result = action(db);
                    if (result.IsSuccess)
                    {
                        trans.Commit();
                    }
                    else
                    {
                        trans.Rollback();
                    }
                    return result;
                }
            }
        }
        #endregion

        static KeKeSoftPlatformDbContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<KeKeSoftPlatformDbContext, KeKeSoftPlatform.Db.Migrations.Configuration>());
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var newException = new FormattedDbEntityValidationException(ex);
                throw newException;
            }
        }

        private string StackTrace(StackFrame[] stacks)
        {
            string result = string.Empty;
            foreach (StackFrame stack in stacks)
            {
                result += string.Format("{0} {1} {2} {3}\r\n", stack.GetFileName(),
                    stack.GetFileLineNumber(),
                    stack.GetFileColumnNumber(),
                    stack.GetMethod().ToString());
            }
            return result;
        }

        public KeKeSoftPlatformDbContext()
            : base("KeKeSoftPlatform")
        {

        }

        private Service _Service;
        public Service Service
        {
            get
            {
                if (this._Service == null)
                {
                    this._Service = new Service(this);
                }
                return this._Service;
            }
        }

        #region 权限
        public DbSet<T_MenuCategory> MenuCategory { get; set; }
        public DbSet<T_Menu> Menu { get; set; }
        public DbSet<T_MenuAction> MenuAction { get; set; }
        
        #endregion

        public DbSet<T_User> User { get; set; }
        public DbSet<T_DaiKou> DaiKou { get; set; }
        public DbSet<T_UserImportRecord> UserImportRecord { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<T_User>()
                        .HasMany(m => m.DaiKous)
                        .WithRequired(m => m.User)
                        .HasForeignKey(m => m.UserId);

            modelBuilder.Entity<T_User>()
                        .HasMany(m => m.UserImportRecords)
                        .WithRequired(m => m.User)
                        .HasForeignKey(m => m.UserId);

            #region 权限

            modelBuilder.Entity<T_MenuCategory>()
                       .HasMany(m => m.Menus)
                       .WithOptional(m => m.MenuCategory)
                       .HasForeignKey(m => m.MenuCategoryId)
                       .WillCascadeOnDelete(false);

            modelBuilder.Entity<T_MenuCategory>()
                        .HasMany(m => m.Children)
                        .WithOptional(m => m.Parent)
                        .HasForeignKey(m => m.ParentId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<T_Menu>()
                       .HasMany(m => m.MenuActions)
                       .WithRequired(m => m.Menu)
                       .HasForeignKey(m => m.MenuId);

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
