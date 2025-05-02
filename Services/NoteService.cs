using KitapOkumaAPI.Data;
using KitapOkumaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KitapOkumaAPI.Services
{
	public class NoteService : INoteService
	{
		private readonly AppDbContext _context;

		public NoteService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<Note>> GetNotesAsync()
		{
			return await _context.Notes.Include(n => n.Book).ToListAsync();
		}

		public async Task<Note?> GetNoteByIdAsync(int id)
		{
			return await _context.Notes.FindAsync(id);
		}

		public async Task<Note> AddNoteAsync(Note note)
		{
			_context.Notes.Add(note);
			await _context.SaveChangesAsync();
			return note;
		}

		public async Task<bool> UpdateNoteAsync(Note note)
		{
			_context.Notes.Update(note);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteNoteAsync(int id)
		{
			var note = await _context.Notes.FindAsync(id);
			if (note == null) return false;

			_context.Notes.Remove(note);
			return await _context.SaveChangesAsync() > 0;
		}
	}
}
