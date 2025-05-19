namespace ContosoUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddThreeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Course",
                c => new
                {
                    CourseID = c.Int(nullable: false),
                    Title = c.String(),
                    Credits = c.Int(nullable: false),
                })
               .PrimaryKey(t => t.CourseID);

            CreateTable(
                "dbo.Enrollment",
                c => new
                {
                    EnrollmentID = c.Int(nullable: false, identity: true),
                    CourseID = c.Int(nullable: false),
                    StudentID = c.Int(nullable: false),
                    Grade = c.Int(),
                })
               .PrimaryKey(t => t.EnrollmentID)
               .ForeignKey("dbo.Course", t => t.CourseID, cascadeDelete: true)
               .ForeignKey("dbo.Student", t => t.StudentID, cascadeDelete: true)
               .Index(t => t.CourseID)
               .Index(t => t.StudentID);

            CreateTable(
                "dbo.Student",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    LastName = c.String(),
                    FirstMidName = c.String(),
                    EnrollmentDate = c.DateTime(nullable: false),
                })
               .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.tb_user",
                c => new
                {
                    uid = c.Int(nullable: false, identity: true),
                    name = c.String(nullable: false),
                    password = c.String(nullable: false),
                    address = c.String(),
                    tel = c.String(),
                    email = c.String()
                })
               .PrimaryKey(t => t.uid);
        }

        public override void Down()
        {
            DropTable("dbo.tb_user");
            DropForeignKey("dbo.Enrollment", "StudentID", "dbo.Student");
            DropForeignKey("dbo.Enrollment", "CourseID", "dbo.Course");
            DropIndex("dbo.Enrollment", new[] { "StudentID" });
            DropIndex("dbo.Enrollment", new[] { "CourseID" });
            DropTable("dbo.Student");
            DropTable("dbo.Enrollment");
            DropTable("dbo.Course");
        }
    }
}