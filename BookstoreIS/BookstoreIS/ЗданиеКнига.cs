//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookstoreIS
{
    using System;
    using System.Collections.Generic;
    
    public partial class ЗданиеКнига
    {
        public int КодЗдания { get; set; }
        public string ISBN { get; set; }
        public string ЦенаКниги { get; set; }
    
        public virtual ТаблицаЗданий ТаблицаЗданий { get; set; }
        public virtual ТаблицаКниг ТаблицаКниг { get; set; }
    }
}
