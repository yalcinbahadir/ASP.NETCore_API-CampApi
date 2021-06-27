using CampApi.Data;
using CampApi.Entities;
using CampApi.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampApi.Repositories.Concrete
{
    public class CampRepository : ICampRepository
    {
        private ApplicationDbContext _context;

        public CampRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        //General
        public   void Add<T>(Task entity) where T : class
        {
           //await  _context.Add<T>(entity);
        }

        public async Task<bool> AddCamp(Camp entity)
        {
            await  _context.Camps.AddAsync(entity);
            return await _context.SaveChangesAsync()>0;
        }

        public async Task<bool>  Delete (Camp camp) 
        {
            _context.Camps.Remove(camp);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }


        //Camps
        public async Task<Camp[]> GetAllCampsAsync(bool includeTalks = false)
        {
           var query = _context.Camps.Include(c => c.Location).AsQueryable();

            if (includeTalks)
            {
                query = query
                          .Include(c => c.Talks)
                          .ThenInclude(t => t.Speaker);
            }

            query = query.OrderByDescending(c => c.EventDate);

            return await query.ToArrayAsync();
           
        }

        public async Task<Camp> GetCampAsync(string moniker, bool includeTalks = false)
        {
            var query =  _context.Camps.Include(c=>c.Location).AsQueryable();
            if (includeTalks)
            {
                query = query
                          .Include(c => c.Talks)
                          .ThenInclude(t => t.Speaker);
            }

            query = query.Where(c => c.Moniker == moniker);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<Camp[]> GetAllCampsByEventDate(DateTime dateTime, bool includeTalks = false)
        {
            var query = _context.Camps.Include(c => c.Location).Where(c=>c.EventDate==dateTime).AsQueryable();

            if (includeTalks)
            {
                query = query
                          .Include(c => c.Talks)
                          .ThenInclude(t => t.Speaker);
            }

            query = query.OrderByDescending(c => c.EventDate);

            return await query.ToArrayAsync();
        }

        public Task<Speaker[]> GetAllSpeakersAsync(string moniker)
        {
            throw new NotImplementedException();
        }


        //Talks

        public async Task<Talk> GetTalkByMonikerAsync(string moniker, int talkId, bool includeSpeakers = false)
        {
            var query =  _context.Talks.Include(t=>t.Camp).AsQueryable();
            
            if (includeSpeakers)
            {
                query = _context.Talks.Include(t => t.Speaker).Include(t => t.Camp).AsQueryable();
            }
            query = query.Where(t => t.Camp.Moniker == moniker);
            return await query.FirstOrDefaultAsync(c => c.TalkId == talkId);
           
        }

        public async Task<Talk[]> GetTalksByMonikerAsync(string moniker, bool includeSpeakers = false)
        {
            var query = _context.Talks.Include(t => t.Camp).AsQueryable();
            if (includeSpeakers)
            {
                query = query.Include(t => t.Speaker);
            }

            return await query.Where(t => t.Camp.Moniker == moniker).ToArrayAsync();
           
        }

        public async Task<bool> AddTalk(Talk entity)
        {
            await _context.Talks.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTalk(Talk talk)
        {
            _context.Talks.Remove(talk);
            return await _context.SaveChangesAsync() > 0;
        }
        // Speakers
        public Task<Speaker[]> GetSpeakersByMonikerAsync(string moniker)
        {
            throw new NotImplementedException();
        }

        public Task<Speaker> GetSpeakerAsync(int speakerId)
        {
            return _context.Speakers.FirstOrDefaultAsync(s=>s.SpeakerId==speakerId);
        }

        public async Task<Camp> GetUpdateAsync(Camp entity)
        {
            var campToUpdate = await GetCampAsync(entity.Moniker, true);
            campToUpdate.Moniker = entity.Moniker;
            campToUpdate.Name = entity.Name;
          
            campToUpdate.EventDate = entity.EventDate;
            campToUpdate.Length = entity.Length;
            if (entity.Location !=null)
            {
                campToUpdate.Location = entity.Location;
            }
            await _context.SaveChangesAsync();
            return campToUpdate;

           ;
        }
    }
}
