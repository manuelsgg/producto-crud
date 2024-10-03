import { useState, useEffect } from "react";
import axios from "axios";

export function DocumentoCrud() {
    // Estados para manejar documentos y la carga de archivos
    const [documents, setDocuments] = useState([]);
    const [file, setFile] = useState(null);
    const [documentName, setDocumentName] = useState("");
    const [editId, setEditId] = useState(null);
    const [loading, setLoading] = useState(false);

    const apiBaseUrl = "http://localhost:5127/api/documento";

    const token = localStorage.getItem('token');

    // Obtener todos los documentos
    const fetchDocuments = async () => {
        try {
            const response = await axios.get(`${apiBaseUrl}`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setDocuments(response.data);
        } catch (error) {
            console.error("Error fetching documents:", error);
        }
    };

    // Subir archivo (crear documento)
    const uploadDocument = async () => {
        if (!file || !documentName) return;

        const formData = new FormData();
        formData.append("file", file);
        formData.append("nombreDocumento", documentName);

        try {
            setLoading(true);
            await axios.post(`${apiBaseUrl}/upload`, formData, {
                headers: {
                    "Content-Type": "multipart/form-data",
                    Authorization: `Bearer ${token}`
                },
            });
            setFile(null);
            setDocumentName("");
            fetchDocuments(); // Actualizar la lista de documentos
        } catch (error) {
            console.error("Error uploading document:", error);
        } finally {
            setLoading(false);
        }
    };

    // Eliminar documento por ID
    const deleteDocument = async (id) => {
        try {
            await axios.delete(`${apiBaseUrl}/${id}`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            fetchDocuments();
        } catch (error) {
            console.error("Error deleting document:", error);
        }
    };


    // Descargar archivo
    const descargarArchivo = async (id, nombre, tipo) => {
        try {
            const response = await axios.get(`${apiBaseUrl}/${id}/download`, {
                responseType: 'blob',
                headers: { Authorization: `Bearer ${token}` }
            });

            console.log('RESPONSE:::::', response);

            // Crear un enlace temporal para descargar el archivo
            const fileURL = window.URL.createObjectURL(new Blob([response.data]));
            const fileLink = document.createElement('a');

            // Configura el nombre del archivo para la descarga
            const fileName = nombre+''+tipo;

            fileLink.href = fileURL;
            fileLink.setAttribute('download', fileName); // Nombre del archivo descargado
            document.body.appendChild(fileLink);

            fileLink.click(); // Ejecuta la descarga

            document.body.removeChild(fileLink); // Limpia el enlace temporal
        } catch (error) {
            console.error('Error downloading file:', error);
        }

    };


    // Manejar la carga de archivos
    const handleFileChange = (e) => {
        setFile(e.target.files[0]);
    };

    // Manejar la carga inicial de los documentos
    useEffect(() => {
        fetchDocuments();
    }, []);

    return (
        <div>
            <h1>Documentos</h1>
            <div>
                <h2>Subir Documento</h2>
                <input
                    type="text"
                    placeholder="Nombre del documento"
                    value={documentName}
                    onChange={(e) => setDocumentName(e.target.value)}
                />
                <input type="file" onChange={handleFileChange} />
                <button onClick={uploadDocument} disabled={loading}>
                    {loading ? "Subiendo..." : "Subir Documento"}
                </button>
            </div>

            <h2>Lista de Documentos</h2>
            {documents.length === 0 ? (
                <p>No hay documentos disponibles</p>
            ) : (
                <table>
                    <thead>
                    <tr>
                        <th>ID</th>
                        <th>Nombre</th>
                        <th>Tipo</th>
                        <th>Acciones</th>
                    </tr>
                    </thead>
                    <tbody>
                    {documents.map((document) => (
                        <tr key={document.documentoId}>
                            <td>{document.documentoId}</td>
                            <td>{document.nombreDocumento}</td>
                            <td>{document.tipo}</td>
                            <td>
                                <button onClick={() => descargarArchivo(document.documentoId, document.nombreDocumento, document.tipo)}>
                                    Descargar Archivo
                                </button>
                                <button onClick={() => deleteDocument(document.documentoId)}>
                                    Eliminar
                                </button>
                            </td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};

