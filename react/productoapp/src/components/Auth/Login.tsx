import { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import {appsettings} from "../../settings/appsettings.ts";
import Swal from "sweetalert2";

export function Login() {
    const [Email, setEmail] = useState('');
    const [Password, setPassword] = useState('');
    const navigate = useNavigate();

    const handleLogin = async () => {
        try {
            const response = await axios.post(`${appsettings.apiUrl}/auth/login`, { Email, Password });
            localStorage.setItem('token', response.data.token);
            const role = response.data.role;
            if (role === 'vendedor') {
                navigate('/vendedor');
            } else if (role === 'cliente') {
                navigate('/cliente');
            }
        } catch (error) {
            console.error('Error en el login:', error);
            await Swal.fire({
                icon: 'error',
                title: 'Error de autenticación',
                text: 'Email o contraseña incorrectos',
            });
        }
    };

    return (
        <div className="container d-flex justify-content-center align-items-center" style={{height: '100vh'}}>
            <div className="card p-4 shadow" style={{width: '400px'}}>
                <h1 className="text-center mb-4">Iniciar Sesión</h1>
                <div className="form-group mb-3">
                    <input
                        type="text"
                        placeholder="Email"
                        className="form-control"
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </div>
                <div className="form-group mb-3">
                    <input
                        type="password"
                        placeholder="Contraseña"
                        className="form-control"
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </div>
                <button className="btn btn-primary btn-block" onClick={handleLogin}>Login</button>
            </div>
        </div>
    );
}