
namespace Wpf.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DocumentRepository : IRepository<Document>
    {
        private StorageEntities2 context = new StorageEntities2();

        public IEnumerable<Document> GetAll()
        {
            foreach (var document in context.Documents)
            {
                yield return document;
            }
        }

        public Document GetById(int id)
        {
            return context.Documents.FirstOrDefault(x => x.Id == id);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Add(Document entity)
        {
            context.Documents.Add(entity);
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
