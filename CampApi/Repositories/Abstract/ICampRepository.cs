using CampApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampApi.Repositories.Abstract
{
    public interface ICampRepository
    {
        //General
        void Add<T>(Task entity) where T : class;
        Task<bool> Delete(Camp camp);
        Task<bool> SaveChangesAsync();

        //Camps
        Task<Camp[]> GetAllCampsAsync(bool includeTalks = false);
        Task<Camp> GetCampAsync(string moniker, bool includeTalks=false);
        Task<Camp[]> GetAllCampsByEventDate(DateTime dateTime, bool includeTalks = false);
        Task<bool> AddCamp(Camp entity);
        Task<Camp> GetUpdateAsync(Camp entity);
        //Talks
        Task<Talk[]> GetTalksByMonikerAsync(string moniker, bool includeSpeakers = false);
        Task<Talk> GetTalkByMonikerAsync(string moniker, int talkId, bool includeSpeakers=false);
        Task<bool> AddTalk(Talk entity);
        Task<bool> DeleteTalk(Talk talk);

        //Speakers
        Task<Speaker[]> GetSpeakersByMonikerAsync(string moniker);
        Task<Speaker> GetSpeakerAsync(int speakerId);
        Task<Speaker[]> GetAllSpeakersAsync(string moniker);
      
    }
}
