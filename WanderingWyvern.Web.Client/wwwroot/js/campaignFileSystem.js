let campaignDirectory = null;
const fileHandles = new Map();
const objectUrls = new Map();
const directoryHandleDatabaseName = "wandering-wyvern";
const directoryHandleStoreName = "settings";
const directoryHandleKey = "campaign-directory";

export async function chooseFolder() {
    if (!window.showDirectoryPicker) {
        throw new Error("Deze browser ondersteunt geen lokale maptoegang.");
    }

    const selectedDirectory = await window.showDirectoryPicker({ mode: "readwrite" });
    clearObjectUrls();
    campaignDirectory = selectedDirectory;
    fileHandles.clear();
    await saveDirectoryHandle(selectedDirectory);
    return await indexDirectory(campaignDirectory, "");
}

export async function hasSavedFolder() {
    if (!window.indexedDB) {
        return false;
    }

    return await loadDirectoryHandle() !== null;
}

export async function restoreFolder(requestPermission = false) {
    if (!window.indexedDB) {
        return null;
    }

    const savedDirectory = await loadDirectoryHandle();
    if (!savedDirectory) {
        return null;
    }

    let permission = await savedDirectory.queryPermission({ mode: "readwrite" });
    if (permission !== "granted" && requestPermission) {
        permission = await savedDirectory.requestPermission({ mode: "readwrite" });
    }

    if (permission !== "granted") {
        return null;
    }

    clearObjectUrls();
    campaignDirectory = savedDirectory;
    fileHandles.clear();
    return await indexDirectory(campaignDirectory, "");
}

export async function listFiles() {
    ensureFolderSelected();
    fileHandles.clear();
    return await indexDirectory(campaignDirectory, "");
}

export async function getFileMetadata() {
    ensureFolderSelected();
    fileHandles.clear();
    return await collectFileMetadata(campaignDirectory, "");
}

export async function readText(relativePath) {
    const file = await getFile(relativePath);
    return await file.text();
}

export async function createObjectUrl(relativePath) {
    const normalizedPath = normalizeRelativePath(relativePath);
    const existingUrl = objectUrls.get(normalizedPath);
    if (existingUrl) {
        return existingUrl;
    }

    const file = await getFile(relativePath);
    const objectUrl = URL.createObjectURL(file);
    objectUrls.set(normalizedPath, objectUrl);
    return objectUrl;
}

export async function exists(relativePath) {
    if (!campaignDirectory) {
        return false;
    }

    try {
        await getFileHandle(relativePath);
        return true;
    } catch (error) {
        if (error.name === "NotFoundError") {
            return false;
        }

        throw error;
    }
}

export async function writeText(relativePath, content) {
    const handle = await getFileHandle(relativePath, true);
    const writable = await handle.createWritable();
    try {
        await writable.write(content);
    } finally {
        await writable.close();
    }
}

export async function appendText(relativePath, content) {
    const existing = await exists(relativePath)
        ? await readText(relativePath)
        : "";

    await writeText(relativePath, existing + content);
}

export function dispose() {
    clearObjectUrls();
    fileHandles.clear();
    campaignDirectory = null;
}

async function indexDirectory(directoryHandle, prefix) {
    const files = [];

    for await (const [name, handle] of directoryHandle.entries()) {
        const relativePath = prefix ? `${prefix}/${name}` : name;

        if (handle.kind === "file") {
            fileHandles.set(relativePath, handle);
            files.push(relativePath);
        } else {
            files.push(...await indexDirectory(handle, relativePath));
        }
    }

    return files.sort((left, right) => left.localeCompare(right));
}

async function collectFileMetadata(directoryHandle, prefix) {
    const metadata = [];

    for await (const [name, handle] of directoryHandle.entries()) {
        const relativePath = prefix ? `${prefix}/${name}` : name;

        if (handle.kind === "file") {
            const file = await handle.getFile();
            metadata.push({
                relativePath,
                size: file.size,
                lastModified: file.lastModified
            });
        } else {
            metadata.push(...await collectFileMetadata(handle, relativePath));
        }
    }

    return metadata.sort((left, right) => left.relativePath.localeCompare(right.relativePath));
}

async function getFile(relativePath) {
    const handle = await getFileHandle(relativePath);
    return await handle.getFile();
}

async function getFileHandle(relativePath, create = false) {
    ensureFolderSelected();
    const normalizedPath = normalizeRelativePath(relativePath);

    if (fileHandles.has(normalizedPath)) {
        return fileHandles.get(normalizedPath);
    }

    const segments = normalizedPath.split("/");
    const fileName = segments.pop();
    let directory = campaignDirectory;

    for (const segment of segments) {
        directory = await directory.getDirectoryHandle(segment, { create });
    }

    const handle = await directory.getFileHandle(fileName, { create });
    fileHandles.set(normalizedPath, handle);
    return handle;
}

function normalizeRelativePath(path) {
    const segments = path.replaceAll("\\", "/").split("/");
    const normalized = [];

    for (const segment of segments) {
        if (!segment || segment === ".") {
            continue;
        }

        if (segment === "..") {
            throw new Error("Ongeldig pad buiten de campagnemap.");
        }

        normalized.push(segment);
    }

    if (normalized.length === 0) {
        throw new Error("Een relatief bestandspad is vereist.");
    }

    return normalized.join("/");
}

function ensureFolderSelected() {
    if (!campaignDirectory) {
        throw new Error("Kies eerst een campagnemap.");
    }
}

function openDirectoryHandleDatabase() {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open(directoryHandleDatabaseName, 1);
        request.onupgradeneeded = () => {
            request.result.createObjectStore(directoryHandleStoreName);
        };
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}

async function loadDirectoryHandle() {
    const database = await openDirectoryHandleDatabase();
    return await new Promise((resolve, reject) => {
        const transaction = database.transaction(directoryHandleStoreName, "readonly");
        const request = transaction.objectStore(directoryHandleStoreName).get(directoryHandleKey);
        request.onsuccess = () => resolve(request.result ?? null);
        request.onerror = () => reject(request.error);
        transaction.oncomplete = () => database.close();
    });
}

async function saveDirectoryHandle(directoryHandle) {
    if (!window.indexedDB) {
        return;
    }

    const database = await openDirectoryHandleDatabase();
    await new Promise((resolve, reject) => {
        const transaction = database.transaction(directoryHandleStoreName, "readwrite");
        transaction.objectStore(directoryHandleStoreName).put(directoryHandle, directoryHandleKey);
        transaction.oncomplete = resolve;
        transaction.onerror = () => reject(transaction.error);
    });
    database.close();
}

export function clearObjectUrls() {
    for (const objectUrl of objectUrls.values()) {
        URL.revokeObjectURL(objectUrl);
    }

    objectUrls.clear();
}
