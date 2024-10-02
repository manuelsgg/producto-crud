import { useState, useEffect } from 'react';
import axios from 'axios';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter, Input, FormGroup, Label } from 'reactstrap';
import Swal from 'sweetalert2';
import {IProduto} from "../../Interfaces/IProduto.ts";
import {appsettings} from "../../settings/appsettings.ts";
import {ICategoria} from "../../Interfaces/ICategoria.ts";
import { useNavigate } from 'react-router-dom';

export function ProductoDashboard() {
    const [productos, setProductos] = useState<IProduto[]>([]);
    const [nuevoProducto, setNuevoProducto] = useState<IProduto>({ productoId: 0, nombreProducto: '', precio: 0, usuarioCreacion: 0 });
    const [editarProducto, setEditarProducto] = useState<IProduto | null>(null);
    const [modal, setModal] = useState(false);
    const [editModal, setEditModal] = useState(false);
    const [categorias, setCategoria] = useState<ICategoria[]>([]);
    const navigate = useNavigate();


    const token = localStorage.getItem('token');


    const fetchProductos = async () => {
        try {
            const response = await axios.get(`${appsettings.apiUrl}/producto`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setProductos(response.data);
            console.log('REPSONSE:::::::::::', response.data)
        } catch (error) {
            console.log('error: ', error);
            await Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'No se pudieron cargar los prudctos',
            });
        }
    };

    const fetchCategorias = async () => {
        try {
            const response = await axios.get(`${appsettings.apiUrl}/categoria`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setCategoria(response.data);
        } catch (error) {
            console.log('Error al obtener categorias: ', error);
            await Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'No se pudieron cargar las categorias',
            });
        }
    };

    useEffect(() => {
        fetchCategorias();
        fetchProductos();
    }, [token]);


    const toggleModal = () => {
        setModal(!modal);
        if (!modal) {
            setNuevoProducto({ categoriaId: 0, nombreProducto: '', precio: 0 });
        }
    }
    const toggleEditModal = () =>
        setEditModal(!editModal);


    const handleAddProducto = async () => {
        try {
            if(!nuevoProducto.categoriaId){
                Swal.fire("Debe seleccionar una categoria");
                return;
            }
            if(!nuevoProducto.nombreProducto){
                Swal.fire("Debe agregar nombre al producto");
                return;
            }
            await axios.post(`${appsettings.apiUrl}/producto`, nuevoProducto, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setNuevoProducto({ categoriaId: 0, nombreProducto: '', precio: 0 });
            toggleModal();
            await Swal.fire('Producto agregado', 'El producto fue agregada exitosamente', 'success');
            await fetchProductos();
        } catch (error) {
            console.log('Error al agregar producto', error);
            await Swal.fire('Error', 'No se pudo agregar producto', 'error');
        }
    };

    const handleEditProducto = async () => {
        try {
            await axios.put(`${appsettings.apiUrl}/producto/${editarProducto?.productoId}`, editarProducto, {
                headers: { Authorization: `Bearer ${token}` }
            });
            toggleEditModal();
            await Swal.fire('Producto actualizada', 'Producto fue actualizado exitosamente', 'success');
            await fetchProductos();
        } catch (error) {
            console.log('Error al actualizar producto', error);
            await Swal.fire('Error', 'No se pudo actualizar el producto', 'error');
        }
    };

    const handleDeleteProducto = async (productoId?: number) => {
        Swal.fire({
            title: '¿Estás seguro?',
            text: "No podrás deshacer esta acción",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar'
        }).then(async (result) => {
            if (result.isConfirmed) {
                try {
                    await axios.delete(`${appsettings.apiUrl}/producto/${productoId}`, {
                        headers: { Authorization: `Bearer ${token}` }
                    });
                    await Swal.fire('Eliminado', 'El producto ha sido eliminada.', 'success');
                    await fetchProductos();
                } catch (error) {
                    console.log('Error: ', error);
                    await Swal.fire('Error', 'No se pudo eliminar producto', 'error');
                }
            }
        });
    };


    const handleLogout = () => {
        localStorage.clear();
        navigate('/');
    };


    return (
        <div className="container">
            <div className="d-flex justify-content-between align-items-center">
                <h2>Productos | Vendedor</h2>
                <Button color="primary" onClick={handleLogout}>
                    Cerrar Sesión
                </Button>
            </div>
            <table className="table table-striped">
                <thead>
                <tr>
                    <th>Producto</th>
                    <th>Precio</th>
                    <th>Categoria</th>
                    <th>Fecha Creación</th>
                    <th>Acciones</th>
                </tr>
                </thead>
                <tbody>
                {productos.map(producto => (
                    <tr key={producto.productoId}>
                        <td>{producto.nombreProducto}</td>
                        <td>{producto.precio}</td>
                        <td>{producto.cNombreCategoria}</td>
                        <td>{producto.fechaCreacion ? new Date(producto.fechaCreacion).toLocaleDateString('es-PE') : 'Sin fecha'}</td>
                        <td>
                            <Button color="warning" onClick={() => {
                                setEditarProducto(producto);
                                toggleEditModal();
                            }}>
                                Editar
                            </Button>
                        </td>
                        <td>
                            <Button color="danger"
                                    onClick={() => handleDeleteProducto(producto.productoId)}>Eliminar</Button>
                        </td>
                    </tr>
                ))}
                </tbody>
            </table>

            <Button color="primary" className="mt-3" onClick={toggleModal}>Agregar Producto</Button>


            <Modal isOpen={modal} toggle={toggleModal}>
                <ModalHeader toggle={toggleModal}>Agregar Producto</ModalHeader>
                <ModalBody>
                    <FormGroup>
                        <Label for="EstudianteId">ID de la Categoria</Label>
                        <Input type="select"
                               id="productoSelect"
                               value={nuevoProducto.categoriaId}
                               onChange={e => setNuevoProducto({
                                   ...nuevoProducto,
                                   categoriaId: parseInt(e.target.value)
                               })}>
                            <option value="">Seleccione una Categoria</option>
                            {categorias.map(categoria => (
                                <option key={categoria.categoriaId} value={categoria.categoriaId}>
                                    {categoria.nombre}
                                </option>
                            ))}
                        </Input>
                    </FormGroup>
                    <FormGroup>
                        <Label for="productname">Nombre Producto</Label>
                        <Input type="text" id="productname" value={nuevoProducto?.nombreProducto}
                               onChange={e => setNuevoProducto({...nuevoProducto!, nombreProducto: e.target.value})}/>
                    </FormGroup>
                    <FormGroup>
                        <Label for="precio">Precio</Label>
                        <Input type="number" id="precio" value={nuevoProducto.precio}
                               onChange={e => {
                                   setNuevoProducto({...nuevoProducto, precio: parseFloat(e.target.value)})
                               }
                               }/>
                    </FormGroup>
                </ModalBody>
                <ModalFooter>
                    <Button color="primary" onClick={handleAddProducto}>Agregar</Button>{' '}
                    <Button color="secondary" onClick={toggleModal}>Cancelar</Button>
                </ModalFooter>
            </Modal>


            <Modal isOpen={editModal} toggle={toggleEditModal}>
                <ModalHeader toggle={toggleEditModal}>Editar Producto</ModalHeader>
                <ModalBody>
                    <FormGroup>
                        <Label for="studentId"><b>Producto: </b> {editarProducto?.nombreProducto}</Label>

                    </FormGroup>
                    <FormGroup>
                        <Label for="editarNombre"><b>Nombre Producto:</b></Label>
                        <Input type="text" id="editarNombre" value={editarProducto?.nombreProducto}
                               onChange={e => setEditarProducto({...editarProducto!, nombreProducto: e.target.value})}/>
                    </FormGroup>
                    <FormGroup>
                        <Label for="EstudianteId">Categoria</Label>
                        <Input type="select"
                               id="productoSelect"
                               value={editarProducto?.categoriaId}
                               onChange={e => setEditarProducto({
                                   ...editarProducto,
                                   categoriaId: parseInt(e.target.value)
                               })}>
                            {categorias.map(categoria => (
                                <option key={categoria.categoriaId} value={categoria.categoriaId}>
                                    {categoria.nombre}
                                </option>
                            ))}
                        </Input>
                    </FormGroup>
                    <FormGroup>
                        <Label for="editarPrecio"><b>Precio:</b></Label>
                        <Input type="number" id="editarPrecio" value={editarProducto?.precio}
                               onChange={e => setEditarProducto({
                                   ...editarProducto!,
                                   precio: parseFloat(e.target.value)
                               })}/>
                    </FormGroup>
                </ModalBody>
                <ModalFooter>
                    <Button color="primary" onClick={handleEditProducto}>Guardar Cambios</Button>{' '}
                    <Button color="secondary" onClick={toggleEditModal}>Cancelar</Button>
                </ModalFooter>
            </Modal>
        </div>
    );
}

