using System;

namespace TitaService
{
    public class TitaServiceImpl : ITitaService
    {
        public string Id { get; set; }
        public TitaServiceImpl()
        {
            Id = Guid.NewGuid().ToString();
        }

    }
}
