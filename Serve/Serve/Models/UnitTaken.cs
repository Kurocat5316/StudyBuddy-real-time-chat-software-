//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Serve.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UnitTaken
    {
        public int takenId { get; set; }
        public string studentId { get; set; }
        public int unitCode { get; set; }
    
        public virtual Student Student { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
