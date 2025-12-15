import { ModalForm, ProFormText, ProFormNumber, ProFormSwitch } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { message } from 'antd';
import React from 'react';
import { productApi } from '@/services/api';

interface CreateProductFormProps {
  visible: boolean;
  onCancel: () => void;
  onSuccess: () => void;
}

const CreateProductForm: React.FC<CreateProductFormProps> = ({
  visible,
  onCancel,
  onSuccess,
}) => {
  const [messageApi, contextHolder] = message.useMessage();

  return (
    <>
      {contextHolder}
      <ModalForm
        title={<FormattedMessage id="pages.searchTable.new" defaultMessage="New Product" />}
        width="400px"
        visible={visible}
        onVisibleChange={(visible) => {
          if (!visible) onCancel();
        }}
        onFinish={async (value) => {
          try {
            const productData = {
              ...value,
              discontinued: value.discontinued || false,
            };

            const response = await productApi.create(productData);
            if (response.statusCode === 201) {
              messageApi.success('Product created successfully');
              onSuccess();
              return true;
            } else {
              messageApi.error(response.message || 'Failed to create product');
              return false;
            }
          } catch (_error) {
            messageApi.error('Failed to create product');
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

export default CreateProductForm;
