using KitapOkumaAPI.Data;
using KitapOkumaAPI.Dtos;
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

        public async Task<Note> AddNoteAsync(NoteDto noteDto)
        {
            var newNote = new Note
            {
                BookId = noteDto.BookId,
                Content = noteDto.Content,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notes.Add(newNote);
            await _context.SaveChangesAsync();
            return newNote;
        }

        public async Task<bool> UpdateNoteAsync(NoteDto noteDto)
        {
            var existingNote = await _context.Notes.FindAsync(noteDto.Id);
            if (existingNote == null)
                return false;

            existingNote.Content = noteDto.Content;
            existingNote.BookId = noteDto.BookId;

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