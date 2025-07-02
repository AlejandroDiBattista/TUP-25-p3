import axios from 'axios';

// URL que se adapta según el entorno
// En desarrollo usa el proxy, en producción usa la URL directa
const URL_Base = 'https://crudcrud.com/api/ad805d9a6be5428abc0c3e8261c642a4';

export default class RestService {
    constructor(recurso = 'contactos') {
        this.recurso = recurso;
    }

    async create(data) {
        const response = await axios.post(`${URL_Base}/${this.recurso}`, data);
        return response.data;
    }

    async readAll() {
        const response = await axios.get(`${URL_Base}/${this.recurso}`);
        return response.data;
    }

    async read(id = '') {
        const response = await axios.get(`${URL_Base}/${this.recurso}/${id}`);
        return response.data;
    }

    async update(id, data) {
        const {_id, ...updateData} = data; 
        const response = await axios.put(`${URL_Base}/${this.recurso}/${id}`, updateData);
        return response.data;
    }

    async delete(id) {
        const response = await axios.delete(`${URL_Base}/${this.recurso}/${id}`);
        return response.data;
    }
}
