using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PropertyRentalManagement.Models;
using PropertyRentalManagement.helpers;
using System.Data.Entity;

namespace PropertyRentalManagement.Controllers
{
    [Authorize]
    public class SearchController : ApiController
    {
        private PropertyRentalManagementDBEntities db = new PropertyRentalManagementDBEntities();
        // GET: api/Search
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Search/5
        public IHttpActionResult Get(string email)
        {
            try
            {                
                var persons = db.Persons.Include(p => p.User).Where(x => x.Email == email
                            && (x.Role == Enumeration.Role.Manager.ToString()
                                  || x.Role == Enumeration.Role.Tenant.ToString())).ToList();
                return Ok(persons);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        // POST: api/Search
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Search/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Search/5
        public void Delete(int id)
        {
        }
    }
}
