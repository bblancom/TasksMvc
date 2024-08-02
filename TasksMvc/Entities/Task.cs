﻿using System.ComponentModel.DataAnnotations;

namespace TasksMvc.Entities
{
    public class Task
    {
        public int Id { get; set; }
        [StringLength(250)]
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Order { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Step> Steps { get; set;}
        public List<AttachedFile> AttachedFiles { get; set;}
    }
}
