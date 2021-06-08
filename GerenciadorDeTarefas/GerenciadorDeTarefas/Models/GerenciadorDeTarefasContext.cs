using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GerenciadorDeTarefas.Models
{
    public class GerenciadorDeTarefasContext : DbContext
    {
        public GerenciadorDeTarefasContext(DbContextOptions<GerenciadorDeTarefasContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuario { get; set; }
    }
}
