using KitapOkumaAPI.Dtos;
using KitapOkumaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KitapOkumaAPI.Services
{
    public interface INoteService
    {
        Task<List<Note>> GetNotesAsync();
        Task<Note> AddNoteAsync(NoteDto noteDto);
        Task<bool> UpdateNoteAsync(NoteDto noteDto);
        Task<bool> DeleteNoteAsync(int id);
    }
}