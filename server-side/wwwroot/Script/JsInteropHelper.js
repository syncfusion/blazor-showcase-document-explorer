window.setPreview = (val, url) => {

    var ele = document.querySelector("[data-uid='" + val + "']");
    if (ele === null) { return; }
    var image = ele.getElementsByClassName('e-list-img')[0];
    image.setAttribute('src', url);
};
window.setLocalCacheImage = (key, img) => {
    var storage = window.localStorage;
    localStorage.setItem(key, img);
};
window.getLocalCacheImage = (key) => {
    var img = localStorage.getItem('key');
    return img;
};
window.setSpinnerPreview = (val) => {
    var ele = document.querySelector("[data-uid='" + val + "']");
    if (ele === null) { return; }
    var rowDiv = document.createElement('img');
    rowDiv.classList.add('e-list-img');
    rowDiv.setAttribute('src', 'data:image/svg+xml;base64,PCEtLSBCeSBTYW0gSGVyYmVydCAoQHNoZXJiKSwgZm9yIGV2ZXJ5b25lLiBNb3JlIEAgaHR0cDovL2dvby5nbC83QUp6YkwgLS0+Cjxzdmcgd2lkdGg9IjU4IiBoZWlnaHQ9IjU4IiB2aWV3Qm94PSIwIDAgNTggNTgiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CiAgICA8ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPgogICAgICAgIDxnIHRyYW5zZm9ybT0idHJhbnNsYXRlKDIgMSkiIHN0cm9rZT0iI0ZGRiIgc3Ryb2tlLXdpZHRoPSIxLjUiPgogICAgICAgICAgICA8Y2lyY2xlIGN4PSI0Mi42MDEiIGN5PSIxMS40NjIiIHI9IjUiIGZpbGwtb3BhY2l0eT0iMSIgZmlsbD0iIzI4N0JENCI+CiAgICAgICAgICAgICAgICA8YW5pbWF0ZSBhdHRyaWJ1dGVOYW1lPSJmaWxsLW9wYWNpdHkiCiAgICAgICAgICAgICAgICAgICAgIGJlZ2luPSIwcyIgZHVyPSIxLjNzIgogICAgICAgICAgICAgICAgICAgICB2YWx1ZXM9IjE7MDswOzA7MDswOzA7MCIgY2FsY01vZGU9ImxpbmVhciIKICAgICAgICAgICAgICAgICAgICAgcmVwZWF0Q291bnQ9ImluZGVmaW5pdGUiIC8+CiAgICAgICAgICAgIDwvY2lyY2xlPgogICAgICAgICAgICA8Y2lyY2xlIGN4PSI0OS4wNjMiIGN5PSIyNy4wNjMiIHI9IjUiIGZpbGwtb3BhY2l0eT0iMCIgZmlsbD0iIzI4N0JENCI+CiAgICAgICAgICAgICAgICA8YW5pbWF0ZSBhdHRyaWJ1dGVOYW1lPSJmaWxsLW9wYWNpdHkiCiAgICAgICAgICAgICAgICAgICAgIGJlZ2luPSIwcyIgZHVyPSIxLjNzIgogICAgICAgICAgICAgICAgICAgICB2YWx1ZXM9IjA7MTswOzA7MDswOzA7MCIgY2FsY01vZGU9ImxpbmVhciIKICAgICAgICAgICAgICAgICAgICAgcmVwZWF0Q291bnQ9ImluZGVmaW5pdGUiIC8+CiAgICAgICAgICAgIDwvY2lyY2xlPgogICAgICAgICAgICA8Y2lyY2xlIGN4PSI0Mi42MDEiIGN5PSI0Mi42NjMiIHI9IjUiIGZpbGwtb3BhY2l0eT0iMCIgZmlsbD0iIzI4N0JENCI+CiAgICAgICAgICAgICAgICA8YW5pbWF0ZSBhdHRyaWJ1dGVOYW1lPSJmaWxsLW9wYWNpdHkiCiAgICAgICAgICAgICAgICAgICAgIGJlZ2luPSIwcyIgZHVyPSIxLjNzIgogICAgICAgICAgICAgICAgICAgICB2YWx1ZXM9IjA7MDsxOzA7MDswOzA7MCIgY2FsY01vZGU9ImxpbmVhciIKICAgICAgICAgICAgICAgICAgICAgcmVwZWF0Q291bnQ9ImluZGVmaW5pdGUiIC8+CiAgICAgICAgICAgIDwvY2lyY2xlPgogICAgICAgICAgICA8Y2lyY2xlIGN4PSIyNyIgY3k9IjQ5LjEyNSIgcj0iNSIgZmlsbC1vcGFjaXR5PSIwIiBmaWxsPSIjMjg3QkQ0Ij4KICAgICAgICAgICAgICAgIDxhbmltYXRlIGF0dHJpYnV0ZU5hbWU9ImZpbGwtb3BhY2l0eSIKICAgICAgICAgICAgICAgICAgICAgYmVnaW49IjBzIiBkdXI9IjEuM3MiCiAgICAgICAgICAgICAgICAgICAgIHZhbHVlcz0iMDswOzA7MTswOzA7MDswIiBjYWxjTW9kZT0ibGluZWFyIgogICAgICAgICAgICAgICAgICAgICByZXBlYXRDb3VudD0iaW5kZWZpbml0ZSIgLz4KICAgICAgICAgICAgPC9jaXJjbGU+CiAgICAgICAgICAgIDxjaXJjbGUgY3g9IjExLjM5OSIgY3k9IjQyLjY2MyIgcj0iNSIgZmlsbC1vcGFjaXR5PSIwIiBmaWxsPSIjMjg3QkQ0Ij4KICAgICAgICAgICAgICAgIDxhbmltYXRlIGF0dHJpYnV0ZU5hbWU9ImZpbGwtb3BhY2l0eSIKICAgICAgICAgICAgICAgICAgICAgYmVnaW49IjBzIiBkdXI9IjEuM3MiCiAgICAgICAgICAgICAgICAgICAgIHZhbHVlcz0iMDswOzA7MDsxOzA7MDswIiBjYWxjTW9kZT0ibGluZWFyIgogICAgICAgICAgICAgICAgICAgICByZXBlYXRDb3VudD0iaW5kZWZpbml0ZSIgLz4KICAgICAgICAgICAgPC9jaXJjbGU+CiAgICAgICAgICAgIDxjaXJjbGUgY3g9IjQuOTM4IiBjeT0iMjcuMDYzIiByPSI1IiBmaWxsLW9wYWNpdHk9IjAiIGZpbGw9IiMyODdCRDQiPgogICAgICAgICAgICAgICAgPGFuaW1hdGUgYXR0cmlidXRlTmFtZT0iZmlsbC1vcGFjaXR5IgogICAgICAgICAgICAgICAgICAgICBiZWdpbj0iMHMiIGR1cj0iMS4zcyIKICAgICAgICAgICAgICAgICAgICAgdmFsdWVzPSIwOzA7MDswOzA7MTswOzAiIGNhbGNNb2RlPSJsaW5lYXIiCiAgICAgICAgICAgICAgICAgICAgIHJlcGVhdENvdW50PSJpbmRlZmluaXRlIiAvPgogICAgICAgICAgICA8L2NpcmNsZT4KICAgICAgICAgICAgPGNpcmNsZSBjeD0iMTEuMzk5IiBjeT0iMTEuNDYyIiByPSI1IiBmaWxsLW9wYWNpdHk9IjAiIGZpbGw9IiMyODdCRDQiPgogICAgICAgICAgICAgICAgPGFuaW1hdGUgYXR0cmlidXRlTmFtZT0iZmlsbC1vcGFjaXR5IgogICAgICAgICAgICAgICAgICAgICBiZWdpbj0iMHMiIGR1cj0iMS4zcyIKICAgICAgICAgICAgICAgICAgICAgdmFsdWVzPSIwOzA7MDswOzA7MDsxOzAiIGNhbGNNb2RlPSJsaW5lYXIiCiAgICAgICAgICAgICAgICAgICAgIHJlcGVhdENvdW50PSJpbmRlZmluaXRlIiAvPgogICAgICAgICAgICA8L2NpcmNsZT4KICAgICAgICAgICAgPGNpcmNsZSBjeD0iMjciIGN5PSI1IiByPSI1IiBmaWxsLW9wYWNpdHk9IjAiIGZpbGw9IiMyODdCRDQiPgogICAgICAgICAgICAgICAgPGFuaW1hdGUgYXR0cmlidXRlTmFtZT0iZmlsbC1vcGFjaXR5IgogICAgICAgICAgICAgICAgICAgICBiZWdpbj0iMHMiIGR1cj0iMS4zcyIKICAgICAgICAgICAgICAgICAgICAgdmFsdWVzPSIwOzA7MDswOzA7MDswOzEiIGNhbGNNb2RlPSJsaW5lYXIiCiAgICAgICAgICAgICAgICAgICAgIHJlcGVhdENvdW50PSJpbmRlZmluaXRlIiAvPgogICAgICAgICAgICA8L2NpcmNsZT4KICAgICAgICA8L2c+CiAgICA8L2c+Cjwvc3ZnPg==');
    var icon = ele.querySelector('.e-list-icon');
    //Insert the image tag in the place of default image
    icon.parentElement.insertBefore(rowDiv, icon);
    icon.remove();
};
window.revertToIconPreview = (val, iconClass) => {
    var ele = document.querySelector("[data-uid='" + val + "']");
    if (ele === null) { return; }
    var image = ele.getElementsByClassName('e-list-img')[0];
    var iconDiv = document.createElement('div');
    iconDiv.classList.add('e-list-icon');
    iconDiv.classList.add(iconClass);
    //Insert the icon div tag for the spinner image
    image.parentElement.insertBefore(iconDiv, image);
    image.remove();
};
window.setDialogDrag = (val) => {
    var dialog = document.getElementById(val).ej2_instances[0];
    dialog.allowDragging = false;
}
window.showStar = (val, isGrid) => {
    var element = document.querySelector("[data-starid='" + val + "']");
    if ((element === null) || (element.querySelector('.star') !== null)) { return; }
    var rowDiv = document.createElement('span');
    rowDiv.className += 'star sf-icon-Starred';
    if (isGrid) {
        element.querySelector('.e-fe-text').appendChild(rowDiv);
    }
    else {
        if (!element.querySelector('.e-list-icon')) {
            rowDiv.className += ' img';
            element.querySelector('.e-text-content').prepend(rowDiv);
        }
        else {
            element.querySelector('.e-list-icon').appendChild(rowDiv);
        }
    }
};

window.toggleStar = (val, isGrid) => {
    var element = document.querySelector("[data-mapId='" + val + "']");
    if ((element === null)) { return "Not Found"; }
    var rowDiv = document.createElement('span');
    rowDiv.className += 'star sf-icon-Starred';
    if (isGrid) {
        var containerEle = ej.base.closest(element, '.e-row');
        if (containerEle.querySelector('.star') === null) {
            containerEle.querySelector('.e-fe-text').appendChild(rowDiv);
            ej.base.addClass([containerEle], ['e-file-star']);
            return "Add";
        } else {
            ej.base.removeClass([containerEle], ['e-file-star']);
            ej.base.remove(containerEle.querySelector('.star'));
            return "Remove";
        }
    }
    else {
        var containerEle = ej.base.closest(element, '.e-large-icon');
        if (containerEle.querySelector('.star') === null) {
            if (!containerEle.querySelector('.e-list-icon')) {
                rowDiv.className += ' img';
                containerEle.querySelector('.e-text-content').prepend(rowDiv);
            }
            else {
                containerEle.querySelector('.e-list-icon').appendChild(rowDiv);
            }
            ej.base.addClass([containerEle], ['e-file-star']);
            return "Add";
        } else {
            ej.base.removeClass([containerEle], ['e-file-star']);
            ej.base.remove(containerEle.querySelector('.star'));
            return "Remove";
        }
    }
};
window.toggleZipFileManagerVisibility = (id, val) => {
    var element = document.getElementById(id);
    if (val) {
        element.style.display = 'block';
    } else {
        element.style.display = 'none';
    }
};
window.renderThumbnail = (id, images) => {
    var ele = document.getElementById(id);

    for (i = 0; i < 5; i++) {
        var anchor = document.createElement('a');
        anchor.setAttribute('role', 'link');
        anchor.setAttribute('id', 'Page_0');
        var thumbnailDiv = document.createElement('div');
        thumbnailDiv.className = 'e-pv-thumbnail e-pv-thumbnail-column';
        thumbnailDiv.style.marginBottom = '20px';
        var imageDiv = document.createElement('div');
        imageDiv.innerHTML = '<img class="e-pv-thumbnail-image" id="pdfViewer_thumbnail_image_0" src="data:image/png;base64,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">';
        imageDiv.addEventListener('focus', function () {
            imageDiv.className = 'e-pv-thumbnail-focus';
        });
        imageDiv.addEventListener('hover', function () {
            imageDiv.className = 'e-pv-thumbnail-hover';
        });
        imageDiv.addEventListener('click', function () {
            imageDiv.className = 'e-pv-thumbnail-selection';
        });
        thumbnailDiv.appendChild(imageDiv);
        anchor.appendChild(thumbnailDiv);
        ele.appendChild(anchor);
        this.wireEvents(imageDiv);
    }
};
var prevSelectedIndex;
window.gotoThumbnailImage = (ele, index) => {
    if (prevSelectedIndex !== undefined) {
        var simageDiv = ele.getElementsByClassName('Ring_' + prevSelectedIndex)[0];
        simageDiv.classList.remove("e-pv-thumbnail-selection");
        simageDiv.classList.add("e-pv-thumbnail-selection-ring");
    }
    var imageDiv = ele.getElementsByClassName('Ring_' + index)[0];
    imageDiv.classList.add("e-pv-thumbnail-selection");
    imageDiv.scrollIntoView();
    prevSelectedIndex = index;
}
window.selectThumbnail = (ele, item) => {
    var imageDiv = ele.getElementsByClassName('Ring_' + item.pageNumber)[0];
    imageDiv.classList.remove("e-pv-thumbnail-selection-ring");
    imageDiv.classList.add("e-pv-thumbnail-selection");
    imageDiv.scrollIntoView();
}
window.removeThumbnailSelection = (ele, prevIndex) => {
    var imageDiv = ele.getElementsByClassName('Ring_' + prevIndex)[0];
    imageDiv.classList.remove("e-pv-thumbnail-selection");
    imageDiv.classList.add("e-pv-thumbnail-selection-ring");
}
window.thumbnailMouseIn = (ele, item) => {
    var imageDiv = ele.getElementsByClassName('Ring_' + item.pageNumber)[0];
    imageDiv.classList.add("e-pv-thumbnail-hover");
}
window.thumbnailMouseOut = (ele, item) => {
    var imageDiv = ele.getElementsByClassName('Ring_' + item.pageNumber)[0];
    imageDiv.classList.remove("e-pv-thumbnail-hover");
}
var timer;
window.toggleBottomToolbarVisibility = (widthRef, isThumbnailVisible) => {
    var refEle = document.getElementById(widthRef);
    var marginLeft;
    if (refEle) {
        marginLeft = (refEle.clientWidth - 393) / 2;
        if ((widthRef == "PptView" || widthRef == "PdfView") && isThumbnailVisible) {
            var sidbarDivElement = document.getElementById(widthRef + "_sideBarContent");
            if (sidbarDivElement) {
                marginLeft = (refEle.clientWidth - 393 + sidbarDivElement.clientWidth) / 2;
            }
        }
    } else {
        marginLeft = (window.innerWidth - 393) / 2;
    }
    var bottomCntr = document.getElementById("bottom_toolbar_container");
    clearTimeout(timer);
    if (bottomCntr) {
        bottomCntr.style.marginLeft = marginLeft + "px";
        bottomCntr.style.display = "block";
    }
    timer = setTimeout(function () {
        if (bottomCntr) {
            bottomCntr.style.display = "none";
        }
    }, 5000);
}
window.getDocumentEditorHeight = () => {
    return (window.innerHeight - 52) + "px";
}
window.fullScreen = (componentId) => {
    var element = document.getElementById(componentId);
    var requestMethod = element.requestFullScreen || element.webkitRequestFullScreen || element.mozRequestFullScreen || element.msRequestFullScreen;
    if (requestMethod) {
        requestMethod.call(element);
    } else if (typeof window.ActiveXObject !== "undefined") { // Older IE.
        var wscript = new ActiveXObject("WScript.Shell");
        if (wscript !== null) {
            wscript.SendKeys("{F11}");
        }
    }
}
window.onresize = () => {
    var documentEditor = document.getElementById("DocEdit");
    if (documentEditor) {
        documentEditor.ej2_instances[0].resize();
    }
}