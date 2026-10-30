---
_disableAffix: true
---

<style>
  /* 1. Fondo oscuro global e inmersivo que coordina con ReportGenerator */
  html, body {
    background-color: #222222 !important;
    overflow: hidden !important;
    margin: 0 !important;
    padding: 0 !important;
  }

  /* 2. Ocultar la barra de migas de pan blanca y el pie de página de DocFX */
  .subnav, footer {
    display: none !important;
  }

  /* 3. Eliminar márgenes, límites de ancho y rellenos de DocFX */
  .container.body-content,
  .article.row.grid,
  .col-md-12,
  article.content.wrap,
  #_content {
    width: 100% !important;
    max-width: 100vw !important;
    padding: 0 !important;
    margin: 0 !important;
  }

  /* 4. Iframe a pantalla completa de borde a borde (Edge-to-Edge) */
  .coverage-frame {
    width: 100vw !important;
    height: calc(100vh - 50px) !important;
    border: none !important;
    display: block !important;
    margin: 0 !important;
    padding: 0 !important;
    background-color: #222222 !important;
  }
</style>

<iframe id="coverage-frame" class="coverage-frame" frameborder="0"></iframe>

<script>
  // Cache-busting automático: añade un parámetro único de tiempo para forzar al navegador
  // a descargar siempre el reporte más reciente sin recurrir a la caché local de disco.
  (function() {
    var frame = document.getElementById('coverage-frame');
    if (frame) {
      frame.src = 'coverage/index.html?t=' + Date.now();
    }
  })();
</script>

<noscript>
  <iframe class="coverage-frame" src="coverage/index.html" frameborder="0"></iframe>
</noscript>
