import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import UpdateProductForm from '../UpdateProductForm';
import { productApi } from '@/services/api';

jest.mock('@/services/api');

describe('UpdateProductForm', () => {
  beforeEach(() => {
    (productApi.getById as jest.Mock).mockResolvedValue({ statusCode: 200, data: { id: '1', productName: 'Test Product' } });
    (productApi.update as jest.Mock).mockResolvedValue({ statusCode: 200, data: {} });
    (productApi.create as jest.Mock).mockResolvedValue({ statusCode: 201, data: {} });
  });

  it('renders and updates product', async () => {
    render(<UpdateProductForm visible={true} productId={'1'} onCancel={() => {}} onSuccess={() => {}} />);

    await waitFor(() => expect(productApi.getById).toHaveBeenCalledWith('1'));

    const input = await screen.findByLabelText(/Product Name/i);
    expect(input).toBeInTheDocument();
  });

  it('creates new product when no id provided', async () => {
    render(<UpdateProductForm visible={true} onCancel={() => {}} onSuccess={() => {}} />);

    const nameInput = await screen.findByLabelText(/Product Name/i);
    await userEvent.type(nameInput, 'New Product');
    const submit = screen.getByRole('button', { name: /ok|submit|确定/i });
    await userEvent.click(submit);

    await waitFor(() => expect(productApi.create).toHaveBeenCalled());
  });
});
