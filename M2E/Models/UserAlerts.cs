//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace M2E.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserAlerts
    {
        public int Id { get; set; }
        public string userType { get; set; }
        public string username { get; set; }
        public string titleText { get; set; }
        public string dateTime { get; set; }
        public string priority { get; set; }
        public string iconUrl { get; set; }
        public string messageFrom { get; set; }
        public string messageTo { get; set; }
    }
}
