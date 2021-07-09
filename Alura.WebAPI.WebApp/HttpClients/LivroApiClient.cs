using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients {
    public class LivroApiClient {

        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _accessor;

        public LivroApiClient(HttpClient httpClient, IHttpContextAccessor accessor) {
            _httpClient = httpClient;
            _accessor = accessor;
        }

        private void AddBearerToken() {
            var token = _accessor.HttpContext.User.Claims.First(c => c.Type == "Token").Value;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo) {
            AddBearerToken();
            var resposta = await _httpClient.GetAsync($"listasleitura/{tipo}");
            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsAsync<Lista>();
        }

        public async Task<LivroApi> GetLivroAsync(int id) {
            AddBearerToken();
            var resposta = await _httpClient.GetAsync($"livros/{id}");
            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsAsync<LivroApi>();
        }

        public async Task<byte[]> GetCapaLivroAsync(int id) {
            AddBearerToken();
            var resposta = await _httpClient.GetAsync($"livros/{id}/capa");
            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsByteArrayAsync();
        }

        public async Task DeleteLivroAsync(int id) {
            AddBearerToken();
            var resposta = await _httpClient.DeleteAsync($"livros/{id}");
            resposta.EnsureSuccessStatusCode();
        }

        private HttpContent CreateMultipartFormDataContent(LivroUpload model) {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Titulo), "\"titulo\"");
            content.Add(new StringContent(model.Lista.ParaString()), "\"lista\"");

            if (!string.IsNullOrEmpty(model.Subtitulo)) {
                content.Add(new StringContent(model.Subtitulo), "\"subtitulo\"");
            }
            if (!string.IsNullOrEmpty(model.Resumo)) {
                content.Add(new StringContent(model.Resumo), "\"resumo\"");
            }

            if (!string.IsNullOrEmpty(model.Autor)) {
                content.Add(new StringContent(model.Autor), "\"autor\"");
            }

            if(model.Id >= 0) {
                content.Add(new StringContent(model.Id.ToString()), "\"id\"");
            }

            if (model.Capa != null) {
                var imagemContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                imagemContent.Headers.Add("content-type", "imagem/png");
                content.Add(imagemContent, "\"capa\"", "\"capa.png\"");
            }
            return content;
        }

        public async Task PostLivroAsync(LivroUpload model) {
            AddBearerToken();
            var content = CreateMultipartFormDataContent(model);
            var resposta = await _httpClient.PostAsync($"livros", content);
            resposta.EnsureSuccessStatusCode();

        }

        public async Task PutLivroAsync(LivroUpload model) {
            AddBearerToken();
            var content = CreateMultipartFormDataContent(model);
            var resposta = await _httpClient.PutAsync($"livros", content);
            resposta.EnsureSuccessStatusCode();

        }

    }
}
