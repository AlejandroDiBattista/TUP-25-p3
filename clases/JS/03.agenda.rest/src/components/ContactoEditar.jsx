import { useEffect } from 'react';
import { Modal, Button, Group, TextInput, Box, Title } from '@mantine/core';
import { useForm } from '@mantine/form';
import { User } from 'lucide-react';

const ContactoEditar = ({ open, onClose, onSubmit, onDelete, initialData }) => {
  const form = useForm({
    initialValues: {
      nombre: '',
      apellido: '',
      direccion: '',
      telefono: '',
      email: ''
    },
    validate: {
      nombre:   (value) => value.trim().length === 0 ? 'El nombre es obligatorio' : null,
      apellido: (value) => value.trim().length === 0 ? 'El apellido es obligatorio' : null,
      telefono: (value) => value.trim().length === 0 ? 'El teléfono es obligatorio' : null,
      email:    (value) => value && !/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(value) ? 'Email inválido' : null
    }
  });

  useEffect(() => {
    if (initialData) {
      form.setValues({
        nombre:    initialData.nombre || '',
        apellido:  initialData.apellido || '',
        direccion: initialData.direccion || '',
        telefono:  initialData.telefono || '',
        email:     initialData.email || ''
      });
    } else {
      form.reset();
    }
  }, [initialData, open]);

  const handleSubmit = (values) => {
    onSubmit(values);
  };

  return (
    <Modal
      opened={open}
      onClose={onClose}
      title={
        <Group align="center">
          <User size={20} style={{ marginRight: 8 }} />
          <Title order={3} style={{ margin: 0 }}>
            {initialData ? 'Editar contacto' : 'Agregar contacto'}
          </Title>
        </Group>
      }
      centered
      withCloseButton={false}
      padding="lg"
      radius="md"
      overlayProps={{ backgroundOpacity: 0.55, blur: 2 }}
    >
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Group grow mb="md">
          <TextInput
            label="Nombre"
            name="nombre"
            {...form.getInputProps('nombre')}
            required
            autoFocus
          />
          <TextInput
            label="Apellido"
            name="apellido"
            {...form.getInputProps('apellido')}
            required
          />
        </Group>
        
        <TextInput
          label="Dirección"
          name="direccion"
          {...form.getInputProps('direccion')}
          mb="md"
        />
        
        <TextInput
          label="Teléfono"
          name="telefono"
          {...form.getInputProps('telefono')}
          required
          mb="md"
        />
        
        <TextInput
          label="Email"
          name="email"
          {...form.getInputProps('email')}
          mb="md"
        />
        
        <Group mt="xl" style={{ justifyContent: 'flex-end' }}>
          {initialData && (
            <Button color="red" variant="outline" radius="xl" onClick={() => onDelete?.(initialData)} style={{ marginRight: 'auto' }}>
              Borrar
            </Button>
          )}
          <Button variant="default" radius="xl" onClick={onClose}>
            Cancelar
          </Button>
          <Button type="submit" color="blue" radius="xl">
            {initialData ? 'Guardar' : 'Agregar'}
          </Button>
        </Group>
      </form>
    </Modal>
  );
};

export default ContactoEditar;
