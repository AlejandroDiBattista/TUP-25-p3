import { test, expect } from '@playwright/test';

test('flujo completo de compra', async ({ page }) => {
  await page.goto('/');
  
  // Verificar que la página principal carga correctamente
  await expect(page).toHaveTitle(/Catálogo/);
  
  // Verificar que se muestran productos
  const productCards = await page.locator('.card').all();
  expect(productCards.length).toBeGreaterThan(0);
  
  // Agregar un producto al carrito
  await page.locator('.card').first().getByRole('button', { name: 'Agregar al carrito' }).click();
  
  // Verificar que el contador del carrito se actualiza
  await expect(page.locator('.cart-count')).toHaveText('1');
  
  // Ir al carrito
  await page.getByRole('link', { name: 'Carrito' }).click();
  
  // Verificar que estamos en la página del carrito
  await expect(page).toHaveURL(/.*carrito/);
  
  // Verificar que el producto está en el carrito
  await expect(page.locator('table tbody tr')).toHaveCount(1);
  
  // Ir a confirmar compra
  await page.getByRole('button', { name: /Confirmar compra/i }).click();
  
  // Verificar que estamos en la página de confirmación
  await expect(page).toHaveURL(/.*confirmar-compra/);
  
  // Llenar formulario
  await page.getByLabel('Nombre').fill('Test');
  await page.getByLabel('Apellido').fill('User');
  await page.getByLabel('Email').fill('test@example.com');
  
  // Confirmar compra
  await page.getByRole('button', { name: /Procesar Compra/i }).click();
  
  // Verificar que volvemos al catálogo
  await expect(page).toHaveURL('/');
});

test('funcionalidad del carrito', async ({ page }) => {
  await page.goto('/');

  // Agregar dos productos diferentes al carrito
  const cards = await page.locator('.card').all();
  await cards[0].getByRole('button', { name: 'Agregar al carrito' }).click();
  await cards[1].getByRole('button', { name: 'Agregar al carrito' }).click();

  // Ir al carrito
  await page.getByRole('link', { name: 'Carrito' }).click();

  // Verificar que hay dos productos en el carrito
  await expect(page.locator('table tbody tr')).toHaveCount(2);

  // Incrementar cantidad del primer producto
  await page.locator('table tbody tr').first().getByRole('button', { name: '+' }).click();
  
  // Verificar que la cantidad se actualizó
  await expect(page.locator('table tbody tr').first().getByRole('textbox')).toHaveValue('2');

  // Decrementar cantidad del primer producto
  await page.locator('table tbody tr').first().getByRole('button', { name: '-' }).click();
  
  // Verificar que la cantidad se actualizó
  await expect(page.locator('table tbody tr').first().getByRole('textbox')).toHaveValue('1');

  // Eliminar el segundo producto
  await page.locator('table tbody tr').nth(1).getByRole('button', { name: /Eliminar/i }).click();
  await page.getByRole('button', { name: 'Sí' }).click();

  // Verificar que solo queda un producto
  await expect(page.locator('table tbody tr')).toHaveCount(1);

  // Vaciar carrito
  await page.getByRole('button', { name: /Vaciar carrito/i }).click();
  await page.getByRole('button', { name: 'Sí' }).click();

  // Verificar que el carrito está vacío
  await expect(page.getByText('El carrito está vacío')).toBeVisible();
});

test('búsqueda y filtrado de productos', async ({ page }) => {
  await page.goto('/');

  // Buscar un producto existente
  await page.getByPlaceholder('Buscar productos...').fill('Notebook');
  
  // Verificar que se muestran resultados
  const searchResults = await page.locator('.card').all();
  expect(searchResults.length).toBeGreaterThan(0);
  
  // Buscar un producto inexistente
  await page.getByPlaceholder('Buscar productos...').fill('xxxxxxxxxxx');
  
  // Verificar que no hay resultados
  await expect(page.getByText('No se encontraron productos')).toBeVisible();
  
  // Limpiar búsqueda
  await page.getByPlaceholder('Buscar productos...').clear();
  
  // Verificar que se muestran todos los productos nuevamente
  const allProducts = await page.locator('.card').all();
  expect(allProducts.length).toBeGreaterThan(0);
});

test('validaciones del formulario de compra', async ({ page }) => {
  // Ir directo a la página de confirmación sin items
  await page.goto('/confirmar-compra');

  // Verificar que nos redirige al catálogo si no hay items
  await expect(page).toHaveURL('/');

  // Agregar un producto y volver a confirmar compra
  await page.locator('.card').first().getByRole('button', { name: 'Agregar al carrito' }).click();
  await page.getByRole('link', { name: 'Carrito' }).click();
  await page.getByRole('button', { name: /Confirmar compra/i }).click();

  // Intentar confirmar sin llenar el formulario
  await page.getByRole('button', { name: /Confirmar Compra/i }).click();

  // Verificar mensajes de validación
  await expect(page.getByText('El nombre es obligatorio')).toBeVisible();
  await expect(page.getByText('El apellido es obligatorio')).toBeVisible();
  await expect(page.getByText('El email es obligatorio')).toBeVisible();

  // Llenar campos con valores inválidos
  await page.getByLabel('Nombre').fill('a'); // muy corto
  await page.getByLabel('Apellido').fill('b'); // muy corto
  await page.getByLabel('Email').fill('invalid-email'); // email inválido

  // Verificar mensajes de validación específicos
  await expect(page.getByText('El nombre debe tener al menos 2 caracteres')).toBeVisible();
  await expect(page.getByText('El apellido debe tener al menos 2 caracteres')).toBeVisible();
  await expect(page.getByText('El email no tiene un formato válido')).toBeVisible();
});

test('límites de cantidad en el carrito', async ({ page }) => {
  await page.goto('/');

  // Agregar un producto al carrito
  await page.locator('.card').first().getByRole('button', { name: 'Agregar al carrito' }).click();
  await page.getByRole('link', { name: 'Carrito' }).click();

  // Intentar incrementar más allá del límite (10)
  for (let i = 0; i < 15; i++) {
    await page.locator('table tbody tr').first().getByRole('button', { name: '+' }).click();
  }

  // Verificar que aparece el mensaje de error
  await expect(page.getByText('No se pueden agregar más de 10 unidades del mismo producto')).toBeVisible();

  // Verificar que la cantidad no supera 10
  await expect(page.locator('.cantidad')).toHaveText('10');

  // Decrementar hasta 1 y verificar que el botón - está deshabilitado
  for (let i = 0; i < 9; i++) {
    await page.locator('table tbody tr').first().getByRole('button', { name: '-' }).click();
  }

  // Verificar que la cantidad es 1
  await expect(page.locator('.cantidad')).toHaveText('1');

  // Un decremento más debería eliminar el item
  await page.locator('table tbody tr').first().getByRole('button', { name: '-' }).click();

  // Verificar que el carrito está vacío
  await expect(page.getByText('El carrito está vacío')).toBeVisible();
});

test('persistencia del carrito', async ({ page }) => {
  await page.goto('/');

  // Agregar productos al carrito
  const cards = await page.locator('.card').all();
  await cards[0].getByRole('button', { name: 'Agregar al carrito' }).click();
  await cards[1].getByRole('button', { name: 'Agregar al carrito' }).click();

  // Recargar la página
  await page.reload();

  // Verificar que el contador del carrito mantiene el valor
  await expect(page.locator('.badge')).toHaveText('2');

  // Ir al carrito y verificar que los productos siguen allí
  await page.getByRole('link', { name: 'Carrito' }).click();
  await expect(page.locator('table tbody tr')).toHaveCount(2);
});
