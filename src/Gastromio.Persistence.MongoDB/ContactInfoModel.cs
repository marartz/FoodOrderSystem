﻿namespace Gastromio.Persistence.MongoDB
{
    public class ContactInfoModel
    {
        public string Phone { get; set; }
        
        public string Fax { get; set; }
        
        public string WebSite { get; set; }
        
        public string ResponsiblePerson { get; set; }
        
        public string EmailAddress { get; set; }
        
        public string Mobile { get; set; }
        
        public bool OrderNotificationByMobile { get; set; }
    }
}