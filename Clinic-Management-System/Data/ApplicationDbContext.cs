using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
﻿using Clinic_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Management_System.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Package> Packages { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<InternDoctor> InternDoctors { get; set; }
        public DbSet<InternDoctorAttendance> InternDoctorAttendances { get; set; }
        public DbSet<Patient> Patient { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<AppointmentByPatientOrReceptionist> appointmentByPatientOrReceptionist { get; set; }
        public DbSet<Receptionist> Receptionist { get; set; }
        public DbSet<ReceptionistAttendance> ReceptionistAttendance { get; set; }

        public DbSet<ReceptionistCurrentShift> ReceptionistCurrentShifts { get; set; }


        public DbSet<TreatmentSession> treatmentSessions { get; set; }


        public DbSet<Check> Checks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}