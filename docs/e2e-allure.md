---
_disableAffix: true
---

<style>
  /* 1. Fondo inmersivo que coordina con Allure Report */
  html, body {
    background-color: #1e1e1e !important;
    overflow: hidden !important;
    margin: 0 !important;
    padding: 0 !important;
  }

  /* 2. Ocultar la barra de migas de pan y el pie de página de DocFX */
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
  .allure-frame {
    width: 100vw !important;
    height: calc(100vh - 50px) !important;
    border: none !important;
    display: block !important;
    margin: 0 !important;
    padding: 0 !important;
    background-color: #1e1e1e !important;
  }
</style>

<iframe id="allure-frame" class="allure-frame" frameborder="0"></iframe>

<script>
  // Cache-busting automático: añade una marca de tiempo única para asegurar que el navegador
  // cargue siempre el informe Allure más reciente generado en CI/CD sin usar caché antigua.
  (function() {
    var frame = document.getElementById('allure-frame');
    if (frame) {
      frame.src = 'allure-report/index.html?t=' + Date.now();
    }
  })();
</script>

<noscript>
  <iframe class="allure-frame" src="allure-report/index.html" frameborder="0"></iframe>
</noscript>

