using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Data
{
    public class DocumentRepository : IRepository<Document>
    {
        public IEnumerable<Document> GetAll()
        {
            using (StorageEntities context = new StorageEntities())
            {
                return context.Documents.ToList();
            }
        }

        public Document GetById(int id)
        {
            using (StorageEntities context = new StorageEntities())
            {
                return context.Documents.FirstOrDefault(x => x.Id == id);
            }
        }

        public void Save(Document entity)
        {
            using (StorageEntities context = new StorageEntities())
            {
                context.Documents.Add(entity);

                context.SaveChanges();
            }
        }
    }
}
