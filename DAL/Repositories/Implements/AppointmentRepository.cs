using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL.Repositories.Implements
{
    public class AppointmentRepository : IAppointmentRepository
    {
            private readonly DbhospitalContext _context;

            public AppointmentRepository(DbhospitalContext context)
            {
                _context = context;
            }

            // Lấy danh sách Appointment có thể tìm kiếm (không phân trang)
            public async Task<IEnumerable<Appointment>> GetAllAsync(string searchString)
            {
                var query = _context.Appointments
                    .Include(a => a.Doctor)
                    .Include(a => a.Patient)
                    .Include(a => a.Room)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    query = query.Where(a =>
                        a.Doctor.FullName.Contains(searchString)
                    ||
                    a.Patient.PatientName.Contains(searchString));
            }

                return await query
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToListAsync();
            }

            //  Lấy 1 Appointment theo Id
            public async Task<Appointment?> GetByIdAsync(int id)
            {
                return await _context.Appointments
                    .Include(a => a.Doctor)
                    .Include(a => a.Patient)
                    .Include(a => a.Room)
                    .FirstOrDefaultAsync(a => a.AppointmentId == id);
            }
            public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId)
            {
                return await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Room)
                    .Where(a => a.DoctorId == doctorId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToListAsync();
            }
            public async Task UpdateAsync(Appointment appointment)
            {
                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
            }

        public async Task<List<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            return await _context.Appointments
                .Include(a => a.AppointmentServices)
                    .ThenInclude(asv => asv.Service)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
    }
    
