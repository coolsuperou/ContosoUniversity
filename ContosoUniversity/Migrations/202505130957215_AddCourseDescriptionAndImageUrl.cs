namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddCourseDescriptionAndImageUrl : DbMigration
    {
        public override void Up()
        {
            // 移除了对tb_user表的创建代码，因为它已经在之前的迁移中创建过

            AddColumn("dbo.Course", "Description", c => c.String(nullable: false));
            AddColumn("dbo.Course", "ImageUrl", c => c.String());
            AlterColumn("dbo.Course", "Title", c => c.String(nullable: false, maxLength: 50));
        }

        public override void Down()
        {
            AlterColumn("dbo.Course", "Title", c => c.String());
            DropColumn("dbo.Course", "ImageUrl");
            DropColumn("dbo.Course", "Description");
            // 注意：不要删除tb_user表，因为它不是这个迁移创建的
            // DropTable("dbo.tb_user");
        }
    }
}