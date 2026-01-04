window.webportalHotkeys = (function () {
  function registerSearchSlash(inputId) {
    document.addEventListener("keydown", function (e) {
      // If user is typing inside input/textarea, ignore.
      const tag = (e.target && e.target.tagName) ? e.target.tagName.toLowerCase() : "";
      const isTyping = tag === "input" || tag === "textarea" || e.target.isContentEditable;
      if (isTyping) return;

      if (e.key === "/") {
        e.preventDefault();
        const el = document.getElementById(inputId);
        if (el) el.focus();
      }
    });
  }

  return {
    registerSearchSlash: registerSearchSlash
  };
})();
