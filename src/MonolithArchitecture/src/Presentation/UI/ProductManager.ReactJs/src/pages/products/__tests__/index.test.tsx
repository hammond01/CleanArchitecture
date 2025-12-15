import React from 'react';
import { render, waitFor } from '@testing-library/react';
import ProductList from '../index';
import { productApi } from '@/services/api';

jest.mock('@/services/api');

describe('ProductList', () => {
  beforeEach(() => {
    (productApi.getAll as jest.Mock).mockResolvedValue({ statusCode: 200, data: [{ id: '1', productName: 'P1' }] });
  });

  it('fetches and displays products', async () => {
    render(<ProductList />);
    await waitFor(() => expect(productApi.getAll).toHaveBeenCalled());
  });
});
