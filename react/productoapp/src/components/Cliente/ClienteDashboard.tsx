import { useEffect, useState } from 'react';
import axios from 'axios';
import {appsettings} from "../../settings/appsettings.ts";
import Swal from "sweetalert2";
import {IProduto} from "../../Interfaces/IProduto.ts";
import {useNavigate} from "react-router-dom";
import {Button} from "reactstrap";

export function ClienteDashboard() {
    const [productos, setProductos] = useState<IProduto[]>([]);
    const token = localStorage.getItem('token');
    const navigate = useNavigate();


    useEffect(() => {
        const fetchGrades = async () => {
            try {
                const response = await axios.get(`${appsettings.apiUrl}/producto`, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                setProductos(response.data);
            } catch (error) {
                console.error('Error al obtener los productos', error);
                await Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "No se pudo obtener producto!"
                });
            }
        };
        fetchGrades();
    }, [token]);

    const handleLogout = () => {
        localStorage.clear();
        navigate('/');
    };

    return (
        <div className="container">
            <div className="d-flex justify-content-between align-items-center">
                <h2>Productos | Cliente</h2>
                <Button color="primary" onClick={handleLogout}>
                    Cerrar Sesión
                </Button>
            </div>
            <table className="table table-striped">
                <thead>
                <tr>
                    <th>Nombre</th>
                    <th>Precio</th>
                </tr>
                </thead>
                <tbody>
                {productos.map(producto => (
                    <tr key={producto.productoId}>
                        <td>{producto.nombreProducto}</td>
                        <td>{producto.precio}</td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
}