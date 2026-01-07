const API_URL = 'http://localhost:5194/api/Socios';

export default {
    async removeSocio(id) {
        // DELETE /api/Socios/RemoveSocio/{id}
        // or just /api/Socios/{id} based on REST conventions, but user specified RemoveSocio
        // Let's assume the endpoint is exactly as requested: RemoveSocio? check request: "RemoveSocio() del controlador SociosController"
        // Usually controllers map RemoveSocio to DELETE /api/Socios/{id} or /api/Socios/RemoveSocio/{id}
        // I'll try the standard pattern /api/Socios/{id} first if it's a standard controller, 
        // BUT the user was specific. Let's assume /api/Socios/RemoveSocio/{id} or simply /api/Socios/{id} if I can't verify.
        // Looking at SocioUpdateModal: PUT `http://localhost:5194/api/Socios/${id}`. 
        // So DELETE is likely `http://localhost:5194/api/Socios/${id}` or `http://localhost:5194/api/Socios/RemoveSocio/${id}`.
        // I will trust the user prompt "RemoveSocio" might be the action name, but in REST it often maps to DELETE verb on resource. 
        // Let's safe bet on `DELETE /api/Socios/RemoveSocio/${id}` if the user was explicit about the name, 
        // OR `DELETE /api/Socios/${id}` if it's a standard generic controller.
        // Given `SocioUpdateModal` uses `PUT /api/Socios/{id}`, `DELETE /api/Socios/{id}` is the most consistent guess for "RemoveSocio".
        // HOWEVER, I will use the path `/RemoveSocio/${id}` just in case, but standard REST is better.
        // Wait, `SocioUpdateModal` uses `http://localhost:5194/api/Socios/byId/${id}` for GET. That is non-standard.
        // So `RemoveSocio` might be `/api/Socios/RemoveSocio/${id}`.

        const response = await fetch(`${API_URL}/RemoveSocio/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al dar de baja al socio');
        }

        return await response.json();
    }
};
