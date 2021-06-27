using CampApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CampApi.Models
{
    public class CampModel
    {
       
        public CampModel()
        {

        }
        public CampModel(Camp camp)
        {
            Name = camp.Name;
            Moniker = camp.Moniker;
            EventDate = camp.EventDate;
            Length = camp.Length;
            Venue = camp.Location.VenueName;
            LocationAddress1 = camp.Location.Address1;
            LocationAddress2 = camp.Location.Address2;
            LocationAddress3 = camp.Location.Address3;
            LocationCityTown = camp.Location.CityTown;
            LocationStateProvince = camp.Location.StateProvince;
            LocationPostalCode = camp.Location.PostalCode;
            LocationCountry = camp.Location.Country;

            Talks = MapTalks(camp.Talks);

        }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        public string Moniker { get; set; }
       
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        [Range(1,100)]
        public int Length { get; set; } = 1;
        //public Location Location { get; set; }
        public string Venue { get; set; }
        public string LocationAddress1 { get; set; }
        public string LocationAddress2 { get; set; }
        public string LocationAddress3 { get; set; }
        public string LocationCityTown { get; set; }
        public string LocationStateProvince { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }

        //[JsonIgnore]
        public ICollection<TalkModel> Talks { get; set; }

        private ICollection<TalkModel> MapTalks(ICollection<Talk> talks)
        {
            var result = new List<TalkModel>();
            if (talks !=null)
            {
                foreach (var talk in talks)
                {
                    result.Add(new TalkModel
                    {

                        Abstract = talk.Abstract,
                        Level = talk.Level,
                        Title = talk.Title,
                        Speaker = new SpeakerModel
                        {
                            FirstName = talk.Speaker.FirstName,
                            LastName = talk.Speaker.LastName,
                            MiddleName = talk.Speaker.MiddleName,
                            Company = talk.Speaker.Company,
                            CompanyUrl = talk.Speaker.CompanyUrl,
                            BlogUrl = talk.Speaker.BlogUrl,
                            Twitter = talk.Speaker.Twitter,
                            GitHub = talk.Speaker.GitHub
                        }
                    });
                }
            }
          
            return result.ToArray();
        }

    }
}
