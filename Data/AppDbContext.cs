﻿using ApiUsuario.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiUsuario.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options) 
        {

        }

        public DbSet<UsuarioModel> Usuarios { get; set; }
    }
}
