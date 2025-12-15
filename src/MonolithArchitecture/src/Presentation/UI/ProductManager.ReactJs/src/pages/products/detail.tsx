import React, { useEffect, useState } from 'react';
import { PageContainer } from '@ant-design/pro-components';
import { useParams } from 'react-router-dom';
import { Card, Descriptions, Spin } from 'antd';
import { productApi } from '@/services/api';

const ProductDetail: React.FC = () => {
  const { id } = useParams() as { id?: string };
  const [loading, setLoading] = useState(true);
  const [product, setProduct] = useState<any | null>(null);

  useEffect(() => {
    let mounted = true;
    const load = async () => {
      if (!id) return;
      try {
        const res = await productApi.getById(String(id));
        if (mounted && res && res.statusCode === 200) setProduct(res.data);
      } catch (_e) {
        // ignore for now
      } finally {
        if (mounted) setLoading(false);
      }
    };
    load();
    return () => {
      mounted = false;
    };
  }, [id]);

  return (
    <PageContainer>
      <Card>
        {loading ? (
          <Spin />
        ) : (
          <Descriptions title={product?.productName || 'Product Details'} bordered column={1}>
            <Descriptions.Item label="ID">{product?.id}</Descriptions.Item>
            <Descriptions.Item label="Product Name">{product?.productName}</Descriptions.Item>
            <Descriptions.Item label="Unit Price">{product?.unitPrice}</Descriptions.Item>
            <Descriptions.Item label="Units In Stock">{product?.unitsInStock}</Descriptions.Item>
            <Descriptions.Item label="Units On Order">{product?.unitsOnOrder}</Descriptions.Item>
            <Descriptions.Item label="Reorder Level">{product?.reorderLevel}</Descriptions.Item>
            <Descriptions.Item label="Discontinued">{product?.discontinued ? 'Yes' : 'No'}</Descriptions.Item>
          </Descriptions>
        )}
      </Card>
    </PageContainer>
  );
};

export default ProductDetail;
