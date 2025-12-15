import { ModalForm, ProFormText, ProFormNumber, ProFormSwitch } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { message } from 'antd';
import React, { useEffect } from 'react';
import { productApi } from '@/services/api';

interface UpdateProductFormProps {
  visible: boolean;
  onCancel: () => void;
  onSuccess: () => void;
  productId?: string | undefined;
}

const UpdateProductForm: React.FC<UpdateProductFormProps> = ({
  visible,
  onCancel,
  onSuccess,
  productId,
}) => {
  const [messageApi, contextHolder] = message.useMessage();
  const [form] = ModalForm.useForm();

  useEffect(() => {
    let mounted = true;
    if (visible) {
      if (productId) {
        // Load product data
        productApi
          .getById(String(productId))
          .then((response) => {
            if (!mounted) return;
            if (response && response.statusCode === 200) {
              form.setFieldsValue(response.data);
            } else {
              messageApi.error(response?.message || 'Failed to load product data');
            }
          })
          .catch(() => {
            if (!mounted) return;
            messageApi.error('Failed to load product data');
          });
      } else {
        // new product - reset form
        form.resetFields();
      }
    }
    return () => {
      mounted = false;
    };
  }, [visible, productId, form, messageApi]);

  return (
    <>
      {contextHolder}
      <ModalForm
        form={form}
        title={<FormattedMessage id="pages.searchTable.edit" defaultMessage="Edit Product" />}
        width="400px"
        visible={visible}
        onVisibleChange={(visible) => {
          if (!visible) onCancel();
        }}
        onFinish={async (value) => {
          try {
            const payload = {
              ...value,
              discontinued: !!value.discontinued,
            } as any;

            let response: any = null;
            if (productId) {
              // update
              response = await productApi.update(String(productId), payload);
            } else {
              // create
              response = await productApi.create(payload);
            }

            if (response && response.statusCode === 200) {
              messageApi.success(
                productId ? 'Product updated successfully' : 'Product created successfully',
              );
              onSuccess();
              return true;
            }

            messageApi.error(response?.message || 'Failed to save product');
            return false;
          } catch (_error) {
            messageApi.error('Failed to save product');
            return false;
          }
        }}
      >
        <ProFormText
          rules={[
            {
              required: true,
              message: (
                <FormattedMessage
                  id="pages.searchTable.productName"
                  defaultMessage="Product name is required"
                />
              ),
            },
          ]}
          width="md"
          name="productName"
          label={<FormattedMessage id="pages.searchTable.productName" defaultMessage="Product Name" />}
        />

        <ProFormNumber
          width="md"
          name="unitPrice"
          label={<FormattedMessage id="pages.searchTable.unitPrice" defaultMessage="Unit Price" />}
          min={0}
          fieldProps={{ precision: 2 }}
        />

        <ProFormNumber
          width="md"
          name="unitsInStock"
          label={<FormattedMessage id="pages.searchTable.unitsInStock" defaultMessage="Units In Stock" />}
          min={0}
        />

        <ProFormNumber
          width="md"
          name="unitsOnOrder"
          label={<FormattedMessage id="pages.searchTable.unitsOnOrder" defaultMessage="Units On Order" />}
          min={0}
        />

        <ProFormNumber
          width="md"
          name="reorderLevel"
          label={<FormattedMessage id="pages.searchTable.reorderLevel" defaultMessage="Reorder Level" />}
          min={0}
        />

        <ProFormSwitch
          name="discontinued"
          label={<FormattedMessage id="pages.searchTable.discontinued" defaultMessage="Discontinued" />}
        />
      </ModalForm>
    </>
  );
};

export default UpdateProductForm;
