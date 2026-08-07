export function attachReferenceLinks(element, dotNetReference) {
    const handleClick = (event) => {
        if (event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) {
            return;
        }

        const anchor = event.target.closest('a[href^="/document/"]');
        if (!anchor || !element.contains(anchor)) {
            return;
        }

        event.preventDefault();
        const path = decodeURIComponent(new URL(anchor.href, document.baseURI).pathname.slice("/document/".length));
        dotNetReference.invokeMethodAsync("OpenReference", path);
    };

    element.addEventListener("click", handleClick);
    element._campaignReferenceClickHandler = handleClick;
}

export function detachReferenceLinks(element) {
    const handleClick = element._campaignReferenceClickHandler;
    if (!handleClick) {
        return;
    }

    element.removeEventListener("click", handleClick);
    delete element._campaignReferenceClickHandler;
}
