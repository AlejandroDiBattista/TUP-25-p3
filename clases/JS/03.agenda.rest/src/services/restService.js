import axios from 'axios';

// URL original de CrudCrud - Vite interceptará y redirigirá automáticamente en desarrollo
const URL_Base = 'https://crudcrud.com/api/b1da457a990a43f4a91c918833b829c2';

export default class RestService {
    constructor(recurso = 'items') {
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
