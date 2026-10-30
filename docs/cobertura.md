---
_disableAffix: true
---

<style>
  /* Expandir contenedor DocFX a ancho completo para el reporte */
  .container.body-content {
    width: 96% !important;
    max-width: 96vw !important;
    padding-left: 10px !important;
    padding-right: 10px !important;
  }
  .article.row.grid, .col-md-12, article.content.wrap {
    margin: 0 !important;
    padding: 0 !important;
    max-width: 100% !important;
    width: 100% !important;
  }
  .coverage-frame {
    width: 100% !important;
    height: 88vh !important;
    border: 1px solid #30363d !important;
    border-radius: 8px !important;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15) !important;
  }
  .coverage-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 15px;
    flex-wrap: wrap;
    gap: 10px;
  }
  .btn-fullscreen {
    display: inline-block;
    padding: 6px 14px;
    background-color: #1f6feb;
    color: #ffffff !important;
    border-radius: 6px;
    font-size: 14px;
    text-decoration: none !important;
    font-weight: 500;
  }
  .btn-fullscreen:hover {
    background-color: #388bfd;
    color: #ffffff !important;
  }
</style>

<div class="coverage-header">
  <div>
    <h1 style="margin: 0 0 5px 0;">Cobertura de Código</h1>
    <p style="margin: 0; color: #8b949e;">Reporte interactivo generado automáticamente por <strong>Coverlet</strong> y <strong>ReportGenerator</strong>.</p>
  </div>
  <div>
    <a href="coverage/index.html" target="_blank" rel="noopener noreferrer" class="btn-fullscreen">
      ↗ Abrir reporte a pantalla completa
    </a>
  </div>
</div>

<iframe class="coverage-frame" src="coverage/index.html" frameborder="0"></iframe>
