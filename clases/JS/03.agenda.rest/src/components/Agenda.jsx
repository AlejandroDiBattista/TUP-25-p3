import { useState } from 'react';
import { Button, Group, Text, TextInput, Box, Stack, Title, ActionIcon } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { Search, User, Users } from 'lucide-react';
import ContactoEditar from './ContactoEditar';
import ContactoVer from './ContactoVer';
import useCrud from '../hooks/useCrud';


const Agenda = () => {
  const { data: contactos, create, update, remove } = useCrud('contactos');
  const [opened, { open, close }] = useDisclosure(false);
  const [editando, setEditando] = useState(null);
  const [buscar, setBuscar] = useState('');

  // Función para normalizar texto removiendo acentos
  const normalizar = (text) => {
    return text.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase();
  };

  // Filtro de búsqueda dinámico (sin useMemo)
  let filtrado = contactos;
  if (buscar.trim()) {
    const s = normalizar(buscar.trim());
    filtrado = contactos.filter(c =>
      [c.nombre, c.apellido, c.telefono, c.email, c.direccion]
        .filter(Boolean)
        .some(val => normalizar(val).includes(s))
    );
  }

  // Abrir para agregar
  const alAgregar = () => {
    setEditando(null);
    open();
  };

  // Abrir para editar
  const alEditar = (contacto) => {
    setEditando(contacto);
    open();
  };

  // Borrar contacto
  const alBorrar = async (contacto) => {
    if (contacto && contacto._id) {
      await remove(contacto._id);
      close();
    }
  };

  // Guardar contacto (nuevo o editado)
  const alEnviar = async (values) => {
    if (editando && editando._id) {
      await update(editando._id, { ...editando, ...values });
    } else {
      await create(values);
    }
    close();
  };

  
  return (
    <Box maw={600} mt={40} mx="auto">
      {/* Título y botón agregar */}
      <Group justify="space-between" align="center" mb="md">
        <Group>
          <ActionIcon size={48} radius="xl" color="blue" variant="light">
            <Users size={22} />
          </ActionIcon>
          <Title order={2}>Agenda</Title>
        </Group>
        <Button onClick={alAgregar} color="blue" radius="xl">Agregar</Button>
      </Group>

      {/* Buscador */}
      <TextInput
        placeholder="Buscar contacto..."
        leftSection={<Search size={18} />}
        radius="xl"
        value={buscar}
        onChange={e => setBuscar(e.target.value)}
        mb="lg"
      />

      {/* Lista de contactos */}
      <Stack spacing="md">
        {filtrado.length === 0 && (
          <Text c="dimmed" align="center">No hay contactos para mostrar.</Text>
        )}
        {filtrado.map(contacto => (
          <ContactoVer
            key={contacto._id}
            contacto={contacto}
            onClick={() => alEditar(contacto)}
          />
        ))}
      </Stack>

      {/* Diálogo de formulario */}
      <ContactoEditar
        open={opened}
        onClose={close}
        onSubmit={alEnviar}
        onDelete={editando ? alBorrar : undefined}
        initialData={editando}
      />
    </Box>
  );
};

export default Agenda;
