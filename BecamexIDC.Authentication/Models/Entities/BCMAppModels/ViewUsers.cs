using System;

namespace BecamexIDC.Authentication.Models.Entities.BCMAppModels
{
    public partial class ViewUsers
    {
        public long? id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string avatar { get; set; }
        public string cover { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string code { get; set; }

        public string staff_id { get; set; }
        public long? timeline_id { get; set; }
        public string department_id { get; set; }
    }
}
