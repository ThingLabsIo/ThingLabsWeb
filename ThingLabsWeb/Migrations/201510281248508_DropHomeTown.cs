namespace ThingLabsWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropHomeTown : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IoTHubConnectionString", c => c.String());
            AddColumn("dbo.AspNetUsers", "IoTHubEndpoint", c => c.String());
            DropColumn("dbo.AspNetUsers", "Hometown");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Hometown", c => c.String());
            DropColumn("dbo.AspNetUsers", "IoTHubEndpoint");
            DropColumn("dbo.AspNetUsers", "IoTHubConnectionString");
        }
    }
}
