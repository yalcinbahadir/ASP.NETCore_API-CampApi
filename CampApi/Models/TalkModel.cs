using CampApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CampApi.Models
{
    public class TalkModel
    {
        public int TalkId { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Required]
        [StringLength(4000,MinimumLength=20)]
        public string Abstract { get; set; }
        [Range(100,300)]
        public int Level { get; set; }
        public SpeakerModel Speaker { get; set; }

        public TalkModel()
        {

        }

        public TalkModel(Talk t)
        {
            TalkId = t.TalkId;
            Title = t.Title;
            Abstract = t.Abstract;
            Level = t.Level;
            if (t.Speaker !=null)
            {
                Speaker = new SpeakerModel
                {
                     SpeakerId=t.Speaker.SpeakerId,
                    FirstName = t.Speaker.FirstName,
                    LastName = t.Speaker.LastName,
                    MiddleName = t.Speaker.MiddleName,
                    BlogUrl = t.Speaker.BlogUrl,
                    Company = t.Speaker.Company,
                    CompanyUrl = t.Speaker.CompanyUrl,
                    Twitter = t.Speaker.Twitter,
                    GitHub = t.Speaker.GitHub

                };
            }
            
        }
    }
}
