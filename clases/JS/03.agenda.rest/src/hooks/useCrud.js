import { useEffect, useState } from 'react';
import { useListState } from '@mantine/hooks';
import RestService from '../services/restService';

export const useCrud = (recurso = 'contacto') => {
    const [data, dataHandlers] = useListState([]);
    const [service] = useState(() => new RestService(recurso));

    async function loadAll() {
        const resultado = await service.readAll();
        dataHandlers.setState(resultado || []);
    }

    async function create(itemData) {
        const resultado = await service.create(itemData);
        dataHandlers.append(resultado);
        return resultado;
    }

    async function update(id, itemData) {
        const resultado = await service.update(id, itemData);
        const i = data.findIndex(item => item._id === id);
        dataHandlers.setItem(i, resultado);
        return resultado;
    }

    async function remove(id) {
        await service.delete(id);
        const i = data.findIndex(item => item._id === id);
        dataHandlers.remove(i);
    }

    useEffect(() => {
        loadAll();
    }, []);

    return {
        data,
        create,
        update,
        remove
    };
};

export default useCrud;
