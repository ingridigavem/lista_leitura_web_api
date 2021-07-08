using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;


namespace Alura.ListaLeitura.Api.Controlers {
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ListasLeituraController : ControllerBase {

        private readonly IRepository<Livro> _repo;

        public ListasLeituraController(IRepository<Livro> repository) {
            _repo = repository;
        }

        private Lista Crialista(TipoListaLeitura tipo) {
            return new Lista {
                Tipo = tipo.ParaString(),
                Livros = _repo.All.Where(l => l.Lista == tipo).Select(l => l.ToApi()).ToList()
            };
        }

        [HttpGet]
        public IActionResult TodasListas() {    
            Lista paraLer = Crialista(TipoListaLeitura.ParaLer);
            Lista lendo = Crialista(TipoListaLeitura.Lendo);
            Lista lidos = Crialista(TipoListaLeitura.Lidos);

            var colecao = new List<Lista> { paraLer, lendo, lidos };

            return Ok(colecao);
        }

        [HttpGet("{tipo}")]
        public IActionResult RecuperaUmTipoDeLista(TipoListaLeitura tipo) {
            var lista = Crialista(tipo);
            return Ok(lista);
        }
        
    }
}
