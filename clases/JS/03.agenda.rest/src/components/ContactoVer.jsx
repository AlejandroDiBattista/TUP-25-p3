import React from 'react';
import { Card, Group, ActionIcon, Box, Text } from '@mantine/core';
import { User } from 'lucide-react';

const ContactoVer = ({ contacto, onClick }) => (
  <Card
    key={contacto._id}
    shadow="sm"
    padding="md"
    radius="md"
    withBorder
    style={{ cursor: 'pointer', borderLeft: '4px solid #228be6' }}
    onClick={onClick}
  >
    <Group align="flex-start" spacing="md">
      <ActionIcon size={40} radius="xl" color="blue" variant="light">
        <User size={28} />
      </ActionIcon>
      <Box grow>
        <Group spacing="xs">
          <Text weight={500}>{contacto.nombre}</Text>
          <Text weight={500}>{contacto.apellido}</Text>
        </Group>
        {contacto.direccion && (
          <Text size="sm" color="dimmed">{contacto.direccion}</Text>
        )}
        <Group spacing="xs" mt={4}>
          <Text size="sm">📞 {contacto.telefono}</Text>
          {contacto.email && <Text size="sm">✉️ {contacto.email}</Text>}
        </Group>
      </Box>
    </Group>
  </Card>
);

export default ContactoVer;
