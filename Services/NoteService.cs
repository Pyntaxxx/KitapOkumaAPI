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

		public async Task<Note> AddNoteAsync(Note note)
		{
			var newNote = new Note
			{
				BookId = note.BookId,
				Content = note.Content,
				CreatedAt = DateTime.UtcNow 
			};
			_context.Notes.Add(newNote);
			await _context.SaveChangesAsync();
			return newNote;
		}

		public async Task<bool> UpdateNoteAsync(Note note)
		{
			var existingNote = await _context.Notes.FindAsync(note.Id);
			if (existingNote == null)
				return false;
			existingNote.Content = note.Content;

			_context.Notes.Update(existingNote);
			await _context.SaveChangesAsync();
			return true;
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
