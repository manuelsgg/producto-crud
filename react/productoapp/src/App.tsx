import { BrowserRouter, Route, Routes } from "react-router-dom";
import {ClienteDashboard} from "./components/Cliente/ClienteDashboard.tsx";
import {Login} from "./components/Auth/Login.tsx";
import {ProductoDashboard} from "./components/Producto/ProductoDashbaord.tsx";
import {DocumentoCrud} from "./components/Documento/DocumentoDashboard.tsx";

function App() {

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Login/>}/>
        <Route path="/documento" element={<DocumentoCrud/>}/>
        <Route path="/vendedor" element={<ProductoDashboard/>}/>
        <Route path="/cliente" element={<ClienteDashboard/>}/>
      </Routes>
    </BrowserRouter>
  )
}

export default App
